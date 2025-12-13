using System.Text.Json.Serialization;

namespace Chess.Core.Pieces;

/// <inheritdoc cref="Piece" />
public class Pawn(Player player, int timesMoved = 0) : Piece(player, timesMoved), ISpecialRulesPiece, IBlockablePiece
{
    /// <inheritdoc/>
    [JsonIgnore]
    public override string Name => "Pawn";

    /// <inheritdoc/>
    [JsonIgnore]
    public override char GetSymbol => Player == Player.White ? '♙' : '♟';

    /// <inheritdoc/>
    [JsonIgnore]
    public override char GetChar => Player == Player.White ? 'P' : 'p';

    /// <summary>
    /// Special Rule for Pawns to determine if the pawn is moving in the correct direction,
    /// is allowed to skip a square and should en passant capture or promote.
    /// If promoting or capturing en passant returns specialPlyAction.
    /// </summary>
    /// <param name="startPosition">Starting Position.</param>
    /// <param name="endPosition">Ending Position.</param>
    /// <param name="board">The board used.</param>
    /// <param name="specialAction">Returns <c>SpecialPlyAction.CaptureEnPassant</c> or <c>SpecialPlyAction.Promote</c> if applicable.</param>
    /// <returns>If the given move can be made.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If Player is neither Black nor White.</exception>
    public bool SpecialRule(
        Position startPosition, Position endPosition, Board board, out SpecialPlyAction? specialAction)
    {
        specialAction = null;

        var relativeMove = new RelativeMove(startPosition, endPosition);

        // Allowed to skip a Square?
        if (relativeMove.RowDistance == 2 && TimesMoved != 0)
        {
            return false;
        }

        // Capture EnPassant
        var possibleEnPassantCapturePosition = board.GetPossibleEnPassantCapturePosition();
        if (possibleEnPassantCapturePosition == endPosition)
        {
            specialAction = SpecialPlyAction.CaptureEnPassant;
        }

        // Capture diagonal & Move straight
        var targetPiece = board.GetPiece(endPosition);
        if (targetPiece != null || specialAction == SpecialPlyAction.CaptureEnPassant)
        {
            if (relativeMove.ColumnDistance != 1 || relativeMove.RowDistance > 1)
            {
                return false;
            }
        }
        else if (targetPiece == null && relativeMove.ColumnDistance != 0)
        {
            return false;
        }

        // Move in the direction of the Enemy
        switch (Player)
        {
            case Player.White:
                if (relativeMove.RowDirection < 0)
                {
                    return false;
                }

                break;
            case Player.Black:
                if (relativeMove.RowDirection > 0)
                {
                    return false;
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(Player), Player, "Player must be either White or Black");
        }

        // Promote
        if (endPosition.Row == board.Rows - 1 || endPosition.Row == 0)
        {
            specialAction = SpecialPlyAction.Promote;
        }

        return true;
    }

    /// <inheritdoc/>
    public void SpecialMove(ValidMove move, Board board)
    {
    }

    /// <inheritdoc/>
    public void SpecialUndo(ValidMove lastMove, Board board)
    {
    }

    /// <inheritdoc/>
    public override bool IsCorrectMovementPattern(RelativeMove relativeMove)
    {
        return relativeMove is { ColumnDistance: <= 1, RowDistance: <= 2 and not 0 };
    }
}
