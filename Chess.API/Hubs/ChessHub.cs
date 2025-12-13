using Chess.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Chess.API.Hubs;

/// <summary>
/// SignalR Hub that connects users to Games and allows them to make/ undo moves,
/// get possible end-positions and delete games.
///
/// The game number that a user is connected to is based off of the url that the user connects to.
/// a connection to <c>/chessHub/6</c> connects to game 6.
/// </summary>
public class ChessHub : Hub
{
    /// <summary>
    /// Called when a new connection is established with the hub.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        await using BoardContext db = new ();

        var gameIdContext = Context.GetHttpContext()?.Request.RouteValues["GameId"]?.ToString();
        var canParse = int.TryParse(gameIdContext, out var gameId);
        if (!canParse)
        {
            return;
        }

        Context.Items["GameId"] = gameId;
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());

        var boardEntry = await db.Boards.FirstOrDefaultAsync(boardEntry => boardEntry.Id == gameId);
        if (boardEntry == null)
        {
            return;
        }

        var board = new Board(boardEntry);
        _ = board.History.TryPeek(out var lastValidMove);
        var gameMover = new GameMover(board);
        gameMover.UpdateState();

        var lastSimpleMove = lastValidMove != null
            ? new SimpleMove(lastValidMove.StartPosition.ToString(), lastValidMove.EndPosition.ToString())
            : null;

        await Clients.Caller.SendAsync("ReceiveBoard", new BoardResponse { BoardString = board.ToString(), LastMove = lastSimpleMove, GameState = gameMover.State });
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a connection with the hub is terminated. Also Removes user from their Group.
    /// </summary>
    /// <param name="exception">Exception that was the cause of the Disconnect.</param>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var gameIdContext = Context.Items["GameId"]?.ToString();
        if (gameIdContext != null)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameIdContext);
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Called by the client using SignalR. Used to make Moves in the current Game.
    /// </summary>
    /// <returns>A Task with a <see cref="StatusCode"/> pertaining to the method call.</returns>
    public async Task<StatusResponse> SendMove(MoveRequest request)
    {
        if (string.IsNullOrEmpty(request.MoveString))
        {
            return new StatusResponse(StatusCode.RequestError, "No move specified!");
        }

        var gameIdContext = Context.Items["GameId"];
        if (gameIdContext == null)
        {
            return new StatusResponse(StatusCode.ServerError, "Request Failed. GameId is not set!");
        }

        var canParse = int.TryParse(gameIdContext.ToString(), out var gameId);
        if (!canParse)
        {
            return new StatusResponse(StatusCode.ServerError, "Request Failed. GameId cant be parsed!");
        }

        var isParseable = MoveParser.TryParseMove(request.MoveString, out var parsedMove);
        if (!isParseable || parsedMove == null)
        {
            return new StatusResponse(StatusCode.RequestError, "Could not parse move.");
        }

        await using BoardContext db = new ();
        var boardEntry = await db.Boards.FirstOrDefaultAsync(boardEntry => boardEntry.Id == gameId);
        if (boardEntry == null)
        {
            return new StatusResponse(StatusCode.NotFound, "Game Not Found.");
        }

        var board = new Board(boardEntry);
        var game = new GameMover(board);

        var isValidMove = MoveValidator.ValidateCurrentMove(parsedMove, board, out var validMove);
        if (!isValidMove || validMove == null)
        {
            return new StatusResponse(StatusCode.RequestError, "Move is invalid.");
        }

        game.Move(validMove);

        boardEntry.Squares = board.Squares;
        boardEntry.History = board.History;
        boardEntry.Turn = board.Turn;
        boardEntry.ReversibleMoveNumber = board.ReversibleMoveNumber;

        db.Boards.Update(boardEntry);

        await db.SaveChangesAsync();

        var lastSimpleMove = new SimpleMove(validMove.StartPosition.ToString(), validMove.EndPosition.ToString());

        await Clients.Group(gameId.ToString()).SendAsync("ReceiveBoard", new BoardResponse { BoardString = board.ToString(), LastMove = lastSimpleMove, GameState = game.State });

        return new StatusResponse(StatusCode.Success, "Successfully Moved.");
    }

    /// <summary>
    /// Called by the client using SignalR. Used to undo Moves in the current Game.
    /// </summary>
    /// <returns>A Task with a <see cref="StatusCode"/> pertaining to the method call.</returns>
    public async Task<StatusResponse> UndoMove(UndoRequest request)
    {
        var gameIdContext = Context.Items["GameId"];
        if (gameIdContext == null)
        {
            return new StatusResponse(StatusCode.ServerError, "Request Failed. GameId is not set!");
        }

        var canParse = int.TryParse(gameIdContext.ToString(), out var gameId);
        if (!canParse)
        {
            return new StatusResponse(StatusCode.ServerError, "Request Failed. GameId cant be parsed!");
        }

        await using BoardContext db = new ();
        var boardEntry = await db.Boards.FirstOrDefaultAsync(boardEntry => boardEntry.Id == gameId);
        if (boardEntry == null)
        {
            return new StatusResponse(StatusCode.NotFound, "Game Not Found.");
        }

        var board = new Board(boardEntry);
        var game = new GameMover(board);

        game.Undo();

        boardEntry.Squares = board.Squares;
        boardEntry.History = board.History;
        boardEntry.Turn = board.Turn;
        boardEntry.ReversibleMoveNumber = board.ReversibleMoveNumber;
        boardEntry.PastBoardOccurrences = board.PastBoardOccurrences;

        db.Boards.Update(boardEntry);

        await db.SaveChangesAsync();

        _ = board.History.TryPeek(out var lastValidMove);

        var lastSimpleMove = lastValidMove != null
            ? new SimpleMove(lastValidMove.StartPosition.ToString(), lastValidMove.EndPosition.ToString())
            : null;

        await Clients.Group(gameId.ToString()).SendAsync("ReceiveBoard", new BoardResponse { BoardString = board.ToString(), LastMove = lastSimpleMove, GameState = game.State });

        return new StatusResponse(StatusCode.Success, "Undo Successful.");
    }

    /// <summary>
    /// Called by the client using SignalR. Used to delete the currently active Game.
    /// </summary>
    /// <returns>A Task with a <see cref="StatusCode"/> pertaining to the method call.</returns>
    public async Task<StatusResponse> DeleteGame(DeleteRequest request)
    {
        var gameIdContext = Context.Items["GameId"];
        if (gameIdContext == null)
        {
            return new StatusResponse(StatusCode.ServerError, "Request Failed. GameId is not set!");
        }

        var canParse = int.TryParse(gameIdContext.ToString(), out var gameId);
        if (!canParse)
        {
            return new StatusResponse(StatusCode.ServerError, "Request Failed. GameId cant be parsed!");
        }

        await using BoardContext db = new ();
        var boardEntry = await db.Boards.FirstOrDefaultAsync(boardEntry => boardEntry.Id == gameId);
        if (boardEntry == null)
        {
            return new StatusResponse(StatusCode.NotFound, "Game Not Found.");
        }

        db.Remove(boardEntry);

        await db.SaveChangesAsync();
        return new StatusResponse(StatusCode.Success, "Successfully Deleted the Game.");
    }

    /// <summary>
    /// Called by the client using SignalR. Used to determine possible end positions for a piece at a given start position.
    /// </summary>
    /// <returns>A Task with an array of positions represented as strings.</returns>
    public async Task<PossiblePositionsResponse> GetPossiblePositions(PossiblePositionsRequest request)
    {
        if (request.Row < 0 || request.Column < 0)
        {
            return new PossiblePositionsResponse { PossiblePositionStrings = [] };
        }

        var gameIdContext = Context.Items["GameId"];
        if (gameIdContext == null)
        {
            return new PossiblePositionsResponse { PossiblePositionStrings = [] };
        }

        var canParse = int.TryParse(gameIdContext.ToString(), out var gameId);
        if (!canParse)
        {
            return new PossiblePositionsResponse { PossiblePositionStrings = [] };
        }

        await using BoardContext db = new ();
        var boardEntry = await db.Boards.FirstOrDefaultAsync(boardEntry => boardEntry.Id == gameId);
        if (boardEntry == null)
        {
            return new PossiblePositionsResponse { PossiblePositionStrings = [] };
        }

        var board = new Board(boardEntry);
        var game = new GameMover(board);
        var positions = game.GetPossiblePositions(new Position(request.Row, request.Column)).Select(p => p.ToString()).ToArray();

        return new PossiblePositionsResponse { PossiblePositionStrings = positions };
    }
}
