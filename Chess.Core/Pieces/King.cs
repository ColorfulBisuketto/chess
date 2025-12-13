using System.Text.Json.Serialization;

namespace Chess.Core.Pieces;

/// <inheritdoc cref="Piece" />
public class King(Player player, int timesMoved = 0) : Piece(player, timesMoved), ISpecialRulesPiece, IBlockablePiece
{
    /// <inheritdoc/>
    [JsonIgnore]
    public override string Name => "King";

    /// <inheritdoc/>
    [JsonIgnore]
    public override char GetSymbol => Player == Player.White ? '♔' : '♚';

    /// <inheritdoc/>
    [JsonIgnore]
    public override char GetChar => Player == Player.White ? 'K' : 'k';

    /// <summary>
    /// Special Rule for Kings to determine if the special Action is to castle.
    /// </summary>
    /// <param name="startPosition">Starting Position.</param>
    /// <param name="endPosition">Ending Position.</param>
    /// <param name="board">The board used.</param>
    /// <param name="specialAction">Returns <c>SpecialPlyAction.Castle</c> if the king is successfully trying to castle.</param>
    /// <returns>True if all rules are being followed.</returns>
    public bool SpecialRule(
        Position startPosition, Position endPosition, Board board, out SpecialPlyAction? specialAction)
    {
        specialAction = null;

        var relativeMove = new RelativeMove(startPosition, endPosition);

        // Is the King allowed to Castle?
        if (relativeMove.ColumnDistance == 2)
        {
            if (TimesMoved != 0)
            {
                return false; // King has already moved
            }

            var rookPosition = relativeMove.ColumnDirection < 0
                ? startPosition with { Column = 0 }
                : startPosition with { Column = board.Rows - 1 };
            var rook = board.GetPiece(rookPosition);

            if (rook is not { TimesMoved: 0 })
            {
                return false; // Rook has already moved
            }

            var rookEndPosition = startPosition with { Column = startPosition.Column + relativeMove.ColumnDirection };
            var relativeRookMove = new RelativeMove(rookPosition, rookEndPosition);
            if (!rook.IsCorrectMovementPattern(relativeRookMove))
            {
                return false; // Rook is can't move here
            }

            if (rook is IBlockablePiece rookBlockable &&
                rookBlockable.CheckIfPathBlocked(rookPosition, relativeRookMove, board))
            {
                return false; // Rook is blocked
            }

            specialAction = SpecialPlyAction.Castle;
        }

        return true;
    }

    /// <summary>
    /// Takes special actions, while moving, if the current special Action is to Castle.
    /// </summary>
    /// <param name="move">The current Move being made.</param>
    /// <param name="board">The board on which to move.</param>
    public void SpecialMove(ValidMove move, Board board)
    {
        if (move.SpecialPlyAction is not SpecialPlyAction.Castle)
        {
            return;
        }

        var columnDirection = move.EndPosition.Column - move.StartPosition.Column > 0 ? 1 : -1;
        var rookStartPosition = columnDirection == 1
            ? move.StartPosition with { Column = board.Columns - 1 }
            : move.StartPosition with { Column = 0 };
        var rookEndPosition = move.EndPosition with { Column = move.StartPosition.Column + columnDirection };

        var rookPiece = board.GetPiece(rookStartPosition);
        board.SetPiece(rookPiece, rookEndPosition);
        board.SetPiece(null, rookStartPosition);
        if (rookPiece != null)
        {
            rookPiece.TimesMoved++;
        }
    }

    /// <summary>
    /// Takes special actions, while undoing a move, if the special Action was to Castle.
    /// </summary>
    /// <param name="lastMove">The last Move made.</param>
    /// <param name="board">The board on which to undo.</param>
    public void SpecialUndo(ValidMove lastMove, Board board)
    {
        if (lastMove.SpecialPlyAction is not SpecialPlyAction.Castle)
        {
            return;
        }

        var columnDirection = lastMove.EndPosition.Column - lastMove.StartPosition.Column > 0 ? 1 : -1;
        var rookStartPosition = columnDirection == 1
            ? lastMove.StartPosition with { Column = board.Columns - 1 }
            : lastMove.StartPosition with { Column = 0 };
        var rookEndPosition = lastMove.EndPosition with { Column = lastMove.StartPosition.Column + columnDirection };

        var rookPiece = board.GetPiece(rookEndPosition);
        board.SetPiece(null, rookEndPosition);
        board.SetPiece(rookPiece, rookStartPosition);
        if (rookPiece != null)
        {
            rookPiece.TimesMoved--;
        }
    }

    /// <inheritdoc/>
    public override bool IsCorrectMovementPattern(RelativeMove relativeMove)
    {
        return relativeMove is { RowDistance: <= 1, ColumnDistance: <= 2 };
    }
}
