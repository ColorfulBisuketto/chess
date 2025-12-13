using Chess.Core;

namespace Chess.KnightsTour;

/// <summary>
/// Defines methods to be used to solve the Knights tour problem.
/// </summary>
public interface IKnightsTourSolver
{
    /// <summary>
    /// Defines a method used to check if the implementing algorithm can solve the given board.
    /// </summary>
    /// <param name="board">The board on which to solve the problem.</param>
    /// <returns>True if the implementing algorithm can solve the problem on the given board.</returns>
    bool CanSolve(Board board);

    /// <summary>
    /// Defines a method used to call for the solving of the Knights tour problem.
    /// </summary>
    /// <param name="board">The board on which to solve the problem.</param>
    /// <returns>An ordered list of the steps/ moves the knight needs to take to solve the problem.</returns>
    /// <exception cref="NotSupportedException">is thrown when the implementing algorithm cant solve the problem on the given board.</exception>
    List<ValidMove> Solve(Board board);
}
