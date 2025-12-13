using Chess.Core;

namespace Chess.KnightsTour;

/// <summary>
/// Defines Helper methods to be used in the solving of the Knights tour problem.
/// </summary>
public abstract class KnightsTourSolverBase : IKnightsTourSolver
{
    /// <summary>
    /// Checks if there is exactly one knight on the board and nothing else.
    /// </summary>
    /// <param name="board">The board to check.</param>
    /// <returns>False if there is more than one knight or any piece that isn't a knight.</returns>
    public bool CanSolve(Board board)
    {
        var knights = 0;
        foreach (var square in board.AllSquares)
        {
            var piece = board.GetPiece(square);
            if (piece == null)
            {
                continue;
            }

            if (piece.Name != "Knight")
            {
                return false;
            }

            knights++;
        }

        return knights == 1;
    }

    /// <inheritdoc/>
    public abstract List<ValidMove> Solve(Board board);

    /// <summary>
    /// Helper method to get the position of the first knight found on the board.
    /// </summary>
    /// <param name="board">The board on which to search.</param>
    /// <returns>The position of the first knight. Null when no knight found.</returns>
    protected static Position? FindKnight(Board board)
    {
        var knights = board.GetPiecesByName("Knight");
        return knights.Count == 0 ? null : knights.First();
    }
}
