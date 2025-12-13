namespace Chess.Core.Pieces;

/// <summary>
/// Used to define if a Piece can be blocked or if it can 'jump' over other Pieces.
/// </summary>
public interface IBlockablePiece
{
    /// <summary>
    /// Chacks if there are any pieces on the path described by <paramref name="startPosition"/> and <paramref name="relativeMove"/> on a given board.
    /// Returns immediately upon finding an invalid Position or a blocking Piece.
    /// </summary>
    /// <param name="startPosition">Starting point from which the Piece begins moving.</param>
    /// <param name="relativeMove">Describes the relative direction and distance in which to move.</param>
    /// <param name="board">The board on which the moves are supposed to be made.</param>
    /// <returns>False if there is a Piece between the starting position and where the ending position is or if a position is outside the board.</returns>
    public bool CheckIfPathBlocked(Position startPosition, RelativeMove relativeMove, Board board)
    {
        for (var i = 1; i < decimal.Max(relativeMove.RowDistance, relativeMove.ColumnDistance); i++)
        {
            var endPosition = new Position(
                startPosition.Row + (i * relativeMove.RowDirection),
                startPosition.Column + (i * relativeMove.ColumnDirection));
            if (!board.IsValidPosition(endPosition))
            {
                return true;
            }

            if (board.GetPiece(endPosition) != null)
            {
                return true;
            }
        }

        return false;
    }
}
