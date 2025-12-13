using Chess.Core;
using Chess.Core.Pieces;

namespace Chess.KnightsTour;

/// <summary>
/// Implements a <see cref="IKnightsTourSolver"/> that solves the problem recursively and without any optimisations.
/// </summary>
public class BruteKnightsTourSolver : KnightsTourSolverBase
{
    /// <summary>
    /// <para>Solves the Knights tour problem by recursively executing all possible moves that can be made until all squares are visited.</para>
    /// <para>
    /// If the knight cant find a legal next move undo the last move and continue searching.
    /// </para>
    /// </summary>
    /// <param name="board">The board for which to solve the problem.</param>
    /// <returns>An ordered List of all moves made to solve the problem.</returns>
    /// <exception cref="NotSupportedException">is thrown if no solution can be found.</exception>
    public override List<ValidMove> Solve(Board board)
    {
        if (!CanSolve(board))
        {
            throw new NotSupportedException("Can't solve this board");
        }

        var mover = new BasicMover(board);

        var knightPosition = FindKnight(board) !.Value;
        var knightPiece = board.GetPiece(knightPosition);

        HashSet<Position> visited = [knightPosition];

        var solved = SolveRecursive(mover, visited, knightPiece!);
        if (!solved)
        {
            throw new NotSupportedException("Can't solve this board");
        }

        return board.History.ToList();
    }

    private static bool SolveRecursive(BasicMover basicMover, HashSet<Position> visited, Piece knightPiece)
    {
        if (visited.Count == basicMover.Board.Rows * basicMover.Board.Columns)
        {
            return true;
        }

        var currentPosition = visited.Last();
        var possibleMoves = basicMover.GetPossiblePositions(currentPosition);
        var nextMoves = possibleMoves.Except(visited);

        foreach (var move in nextMoves)
        {
            visited.Add(move);
            var validMove = new ValidMove(knightPiece, currentPosition, move, null, null, null);
            basicMover.Move(validMove);
            var result = SolveRecursive(basicMover, visited, knightPiece);
            if (result)
            {
                return true;
            }

            basicMover.Undo();
            visited.Remove(move);
        }

        return false;
    }
}
