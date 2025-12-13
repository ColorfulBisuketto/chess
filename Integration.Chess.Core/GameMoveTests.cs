using System.Reflection;
using System.Text.Json;
using Chess.Core;
using Chess.Core.Pieces;
using Shouldly;

namespace Integration.Chess.Core;

/// <summary>
/// Model to Serialize/ Deserialize example Games.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
internal class GameTestHelper
{
    /// <summary>
    /// Contains a list of Moves to be made in a simplified PGN Notation.
    /// </summary>
    public string MoveList { get; set; }

    /// <summary>
    /// The Board that should be left after all moves have been made in FEN notation.
    /// </summary>
    public string ResultingBoard { get; set; }
}

/// <summary>
/// Integration tests for playing games of chess.
/// </summary>
public class GameMoveTests
{
    [Theory]
    [InlineData("a3")]
    [InlineData("Pe3")]
    [InlineData("c4")]
    [InlineData("d2d4")]
    public void Move_Should_Move_Pawns(string targetPosition)
    {
        // given
        var board = BoardBuilder.StandardGame();
        var game = new GameMover(board);
        var expectedPosition = Position.FromString(targetPosition[^2..]);

        // when
        var res = game.Move(targetPosition);

        // then
        res.ShouldBeTrue();
        board.GetPiece(expectedPosition)?.Name.ShouldBe("Pawn");
    }

    [Fact]
    public void Move_Should_Castle()
    {
        // given
        var kingEndPosition = new Position(0, 6);
        var rookEndPosition = new Position(0, 5);
        var board = BoardBuilder.StandardGame()
            .RemovePiece(kingEndPosition)
            .RemovePiece(rookEndPosition);
        var game = new GameMover(board);

        // when
        var res = game.Move("Kg1");

        // then
        res.ShouldBeTrue();
        board.GetPiece(kingEndPosition)?.Name.ShouldBe("King");
        board.GetPiece(rookEndPosition)?.Name.ShouldBe("Rook");
        var lastMove = board.History.Peek();
        lastMove.ShouldNotBeNull();
        lastMove.SpecialPlyAction.ShouldBe(SpecialPlyAction.Castle);
    }

    [Fact]
    public void Move_Should_EnPassant_Capture()
    {
        // given
        var blackPawn = new Pawn(Player.Black);
        var blackPawnPosition = new Position(3, 3);
        var pawnEndPosition = new Position(2, 3);
        var board = BoardBuilder.StandardGame()
            .AddPiece(blackPawn, blackPawnPosition);
        var game = new GameMover(board);

        // when
        game.Move("c4");
        var res = game.Move("c3");

        // then
        res.ShouldBeTrue();
        board.GetPiece(pawnEndPosition)?.Name.ShouldBe("Pawn");
        board.GetPiece(pawnEndPosition)?.Player.ShouldBe(Player.Black);
        var lastMove = board.History.Peek();
        lastMove.ShouldNotBeNull();
        lastMove.SpecialPlyAction.ShouldBe(SpecialPlyAction.CaptureEnPassant);
        lastMove.CapturedPiece!.Name.ShouldBe("Pawn");
    }

    [Fact]
    public void Move_Should_Promote_Pawns()
    {
        // given
        var pawn = new Pawn(Player.White);
        var pawnPosition = Position.FromString("a7");
        var pawnEndPosition = Position.FromString("a8");
        var board = new Board(8, 8)
            .AddPiece(pawn, pawnPosition);
        var game = new GameMover(board);

        // when
        var res = game.Move("a8=Q");

        // then
        res.ShouldBeTrue();
        board.GetPiece(pawnEndPosition)?.Name.ShouldBe("Queen");
        var lastMove = board.History.Peek();
        lastMove.ShouldNotBeNull();
        lastMove.SpecialPlyAction.ShouldBe(SpecialPlyAction.Promote);
    }

    [Fact]
    public void Move_Should_Allow_Playing_Full_Games()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var @namespace = assembly.GetName().Name;
        const string filename = "TestGames.json";
        using var stream = assembly.GetManifestResourceStream($"{@namespace}.{filename}").ShouldNotBeNull();
        var gamesJson = new StreamReader(stream).ReadToEnd();

        var games = JsonSerializer.Deserialize<GameTestHelper[]>(gamesJson).ShouldNotBeNull();

        foreach (var game in games)
        {
            // given
            var gameMover = GameMover.NewStandardGame();

            // when
            var result = MoveListGenerator.PlayMany(gameMover, game.MoveList, out var failedMove);

            // then
            failedMove.ShouldBeNull();
            result.ShouldBeTrue();
            if (game.ResultingBoard != string.Empty)
            {
                gameMover.Board.ToString().ShouldBe(game.ResultingBoard);
            }
        }
    }

    /// <summary>
    /// Test king moves for "1k3K2/2n5/8/8/8/6Q1/8/8 w - - 1 2", where the king cant move into danger.
    /// </summary>
    [Fact]
    public void King_Should_Not_Be_Allowed_To_Move_Into_Danger()
    {
        // given
        var kingPosition = new Position(7, 5);
        var illegalKingPosition = new Position(7, 4);
        var board = BoardBuilder.EmptyStandardGame()
            .AddPiece(new King(Player.Black), new Position(7, 1))
            .AddPiece(new King(Player.White), kingPosition)
            .AddPiece(new Knight(Player.Black), new Position(6, 2))
            .AddPiece(new Queen(Player.White), new Position(2, 6));
        var game = new GameMover(board);

        // when
        var res = game.GetPossiblePositions(kingPosition);

        // then
        res.ShouldNotContain(illegalKingPosition);
    }

    /// <summary>
    /// Test knight moves for "1k2K3/2k5/8/8/8/6Q1/8/8 b - - 1 2",
    /// The knight is not allowed to move as this would put the king in danger.
    /// </summary>
    [Fact]
    public void Knight_Should_Not_Endanger_Friendly_King()
    {
        // given
        var knightPosition = new Position(6, 2);
        var kingPosition = new Position(7, 5);
        var board = BoardBuilder.EmptyStandardGame()
            .AddPiece(new King(Player.Black), new Position(7, 1))
            .AddPiece(new King(Player.White), kingPosition)
            .AddPiece(new Knight(Player.Black), knightPosition)
            .AddPiece(new Queen(Player.White), new Position(2, 6));
        board.Turn = Player.Black;
        var game = new GameMover(board);

        // when
        var res = game.GetPossiblePositions(knightPosition);

        // then
        res.Length.ShouldBe(0);
    }

    /// <summary>
    /// Test knight moves for "1k2K3/2k5/8/8/8/6Q1/8/8 b - - 1 2"
    /// (Knight moves are impossible to know since boards where the current player can capture a King are illegal).
    /// </summary>
    [Fact]
    public void Knight_Should_Be_Allowed_Undefined_behavior()
    {
        // given
        var knightPosition = new Position(6, 2);
        var kingPosition = new Position(7, 4);
        var board = BoardBuilder.EmptyStandardGame()
            .AddPiece(new King(Player.Black), new Position(7, 1))
            .AddPiece(new King(Player.White), kingPosition)
            .AddPiece(new Knight(Player.Black), knightPosition)
            .AddPiece(new Queen(Player.White), new Position(2, 6));
        board.Turn = Player.Black;
        var game = new GameMover(board);

        // when
        var res = game.GetPossiblePositions(knightPosition);

        // then
        res.ShouldContain(kingPosition);
        res.Length.ShouldBe(1);
    }

    /// <summary>
    /// Test Queen moves for "1k2K3/2n5/8/8/8/6Q1/8/8 w - - 1 2"
    /// Other moves are also possible but the queen should only be allowed to move in a way to stop the threat on the King.
    /// </summary>
    [Fact]
    public void Queen_Should_Capture_Knight_To_Protect_King()
    {
        // given
        var knightPosition = new Position(6, 2);
        var queenPosition = new Position(2, 6);
        var board = BoardBuilder.EmptyStandardGame()
            .AddPiece(new King(Player.Black), new Position(7, 1))
            .AddPiece(new King(Player.White), new Position(7, 4))
            .AddPiece(new Knight(Player.Black), knightPosition)
            .AddPiece(new Queen(Player.White), queenPosition);
        var game = new GameMover(board);

        // when
        var res = game.GetPossiblePositions(queenPosition);

        // then
        res.ShouldContain(knightPosition);
        res.Length.ShouldBe(1);
    }
}
