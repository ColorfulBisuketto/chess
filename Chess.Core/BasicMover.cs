using Chess.Core.Pieces;

namespace Chess.Core;

/// <summary>
/// A basic mover that only moves pieces according to validMoves and specialPlyActions.
/// Decorate this class for more functionalities.
/// </summary>
/// <param name="board">The board to bind to.</param>
public class BasicMover(Board board) : IMover
{
    /// <inheritdoc/>
    public Board Board { get; } = board;

    /// <inheritdoc/>
    public Position[] GetPossiblePositions(Position startPosition)
    {
        var turn = Board.GetPiece(startPosition)?.Player;
        var moves = GetPossibleMoves(startPosition, turn);

        var possiblePositions = moves.Select(item => item.EndPosition).ToHashSet();

        return possiblePositions.ToArray();
    }

    /// <inheritdoc />
    public bool Move(ValidMove validMove)
    {
        Board.SetPiece(
            validMove.SpecialPlyAction == SpecialPlyAction.Promote ? validMove.PromoteToPiece : validMove.Piece,
            validMove.EndPosition);
        Board.SetPiece(null, validMove.StartPosition);

        validMove.Piece.TimesMoved++;
        if (validMove.PromoteToPiece != null)
        {
            validMove.PromoteToPiece.TimesMoved++;
        }

        if (validMove.Piece is ISpecialRulesPiece specialPiece)
        {
            specialPiece.SpecialMove(validMove, Board);
        }

        if (validMove is { SpecialPlyAction: SpecialPlyAction.CaptureEnPassant })
        {
            var hasHistory = Board.History.TryPeek(out var lastMove);
            if (hasHistory && lastMove != null)
            {
                Board.SetPiece(null, lastMove.EndPosition);
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                    nameof(lastMove),
                    "Last move has to exist for an EnPassant Capture to be possible.");
            }
        }

        Board.History.Push(validMove);
        return true;
    }

    /// <inheritdoc/>
    public void Undo()
    {
        var moveExists = Board.History.TryPop(out var lastMove);
        if (!moveExists || lastMove == null)
        {
            return;
        }

        var (piece, previousPosition, currentPosition, capturedPiece, lastAction, _) = lastMove;

        Board.SetPiece(piece, previousPosition);
        piece.TimesMoved--;

        var actualCapturePosition = currentPosition;
        if (lastAction is SpecialPlyAction.CaptureEnPassant)
        {
            var hasHistory = Board.History.TryPeek(out var enPassantMove);
            if (hasHistory && enPassantMove != null)
            {
                actualCapturePosition = enPassantMove.EndPosition;
                Board.SetPiece(null, currentPosition);
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                    nameof(enPassantMove),
                    "enPassantMove has to exist for an EnPassant Capture to have been possible.");
            }
        }

        Board.SetPiece(capturedPiece, actualCapturePosition);

        if (piece is ISpecialRulesPiece specialPiece)
        {
            specialPiece.SpecialUndo(lastMove, Board);
        }
    }

    /// <summary>
    /// Internal Method to get a List of moves that can be made immediately.
    /// </summary>
    /// <param name="startPosition">Starting Position.</param>
    /// <param name="turn">The Player for which to get the moves.</param>
    /// <returns>A HashSet of Moves that can be made immediately.</returns>
    internal HashSet<ValidMove> GetPossibleMoves(Position startPosition, Player? turn)
    {
        var possibleMoves = new HashSet<ValidMove>();
        var piece = Board.GetPiece(startPosition);
        if (piece == null)
        {
            return possibleMoves;
        }

        foreach (var square in Board.AllSquares)
        {
            var unvalidatedMove = new UnvalidatedMove(
                piece.Name,
                startPosition.Row,
                startPosition.Column,
                square.Row,
                square.Column,
                string.Empty);
            var valid = MoveValidator.ValidatePossibleMove(unvalidatedMove, Board, turn, out var move);
            if (!valid || move == null)
            {
                continue;
            }

            possibleMoves.Add(move);
        }

        return possibleMoves;
    }
}
