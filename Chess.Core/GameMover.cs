using Chess.Core.Pieces;

namespace Chess.Core;

/// <summary>
/// A mover for a game that moves pieces according to validMoves and specialPlyActions.
/// Also updates current turn, keeps track of repetition and the number of reversible moves.
/// </summary>
public class GameMover : IMover
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GameMover"/> class for a given board.
    /// </summary>
    /// <param name="board">The board that should be manipulated.</param>
    public GameMover(Board board)
    {
        Board = board;
        BasicMover = new BasicMover(Board);
    }

    /// <inheritdoc/>
    public Board Board { get; }

    /// <summary>
    /// Stores the current State of the game.
    /// Always refers to the player whose turn it is.
    /// </summary>
    public GameState State { get; private set; }

    private BasicMover BasicMover { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GameMover"/> class after creating a new board with the standard pieces.
    /// </summary>
    /// <returns>An instance of the <see cref="GameMover"/> class.</returns>
    public static GameMover NewStandardGame()
    {
        var board = BoardBuilder.StandardGame();
        board.PastBoardOccurrences.Add(string.Join(" ", board.ToString().Split(' ').Take(4)), 1);
        return new GameMover(board);
    }

    /// <inheritdoc/>
    public bool Move(ValidMove validMove)
    {
        if (State is GameState.CheckMate or GameState.Draw)
        {
            return false;
        }

        BasicMover.Move(validMove);
        if (validMove.CapturedPiece != null || validMove.SpecialPlyAction == SpecialPlyAction.CaptureEnPassant ||
            validMove.Piece is Pawn)
        {
            Board.ReversibleMoveNumber = 0;
            Board.PastBoardOccurrences.Clear();
        }
        else
        {
            Board.ReversibleMoveNumber++;
        }

        var trackedBoardString = string.Join(" ", Board.ToString().Split(' ').Take(4));
        Board.PastBoardOccurrences[trackedBoardString] =
            Board.PastBoardOccurrences.TryGetValue(trackedBoardString, out var value) ? value + 1 : 1;

        Board.Turn = Board.Turn switch
        {
            Player.White => Player.Black,
            Player.Black => Player.White,
            _ => Board.Turn,
        };

        UpdateState();

        return true;
    }

    /// <inheritdoc/>
    public void Undo()
    {
        if (Board.History.Count == 0)
        {
            return;
        }

        var trackedBoardString = string.Join(" ", Board.ToString().Split(' ').Take(4));
        var occurrences = Board.PastBoardOccurrences.GetValueOrDefault(trackedBoardString, 0);
        if (occurrences > 0)
        {
            Board.PastBoardOccurrences[trackedBoardString] = occurrences - 1;
        }

        BasicMover.Undo();
        if (Board.ReversibleMoveNumber < 0)
        {
            Board.ReversibleMoveNumber = -1;
        }
        else
        {
            Board.ReversibleMoveNumber--;
        }

        Board.Turn = Board.Turn switch
        {
            Player.White => Player.Black,
            Player.Black => Player.White,
            _ => Board.Turn,
        };

        UpdateState();
    }

    /// <inheritdoc/>
    public Position[] GetPossiblePositions(Position startPosition)
    {
        return BasicMover.GetPossiblePositions(startPosition).ToArray();
    }

    /// <summary>
    /// First validate an unvalidated move before moving all the relevant pieces on the board.
    /// Return before execution if the move is invalid.
    /// </summary>
    /// <param name="unvalidatedMove">A move that has not been validated.</param>
    /// <returns>True if modification of the board was a success and the move was valid.</returns>
    public bool Move(UnvalidatedMove unvalidatedMove)
    {
        var isValid = MoveValidator.ValidateCurrentMove(unvalidatedMove, Board, out var validMove);
        if (!isValid || validMove == null)
        {
            return false;
        }

        return Move(validMove);
    }

    /// <summary>
    /// First parse a movement string from algebraic chess notation then validate the move before moving all the relevant pieces on the board.
    /// Return before execution if the move is invalid or not parseable.
    /// </summary>
    /// <param name="moveString">A move represented in algebraic chess notation as a string.</param>
    /// <returns>True if modification of the board was a success, the move was valid and the move was parseable.</returns>
    public bool Move(string moveString)
    {
        var parseAble = MoveParser.TryParseMove(moveString, out var parsedMove);
        if (!parseAble || parsedMove == null)
        {
            return false;
        }

        return Move(parsedMove);
    }

    /// <summary>
    /// Update the current board state.
    ///
    /// Automatically called on every move/ undo.
    /// </summary>
    public void UpdateState()
    {
        if (Board.Turn != null && MoveValidator.CheckKingInCheckMate(Board, (Player)Board.Turn))
        {
            State = GameState.CheckMate;
        }
        else if (MoveValidator.CheckDeadPosition(Board)
                 || (Board.Turn != null && MoveValidator.CheckStaleMate(Board, (Player)Board.Turn)))
        {
            State = GameState.Draw;
        }
        else if (Board.Turn != null && MoveValidator.CheckKingInCheck(Board, (Player)Board.Turn))
        {
            State = GameState.Check;
        }
        else
        {
            State = GameState.Running;
        }
    }
}

/// <summary>
/// The current State of the game.
/// </summary>
public enum GameState
{
    /// <summary>
    /// The Game is currently is progress, without anything special happening.
    /// </summary>
    Running = 0,

    /// <summary>
    /// The Player Whose turn it is currently is in Check.
    /// </summary>
    Check = 1,

    /// <summary>
    /// The Player Whose turn it is currently is in CheckMate and the Game is over.
    /// </summary>
    CheckMate = 2,

    /// <summary>
    /// The Game is a Draw and the Game is over.
    /// </summary>
    Draw = 3,
}
