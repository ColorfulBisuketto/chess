using Chess.Core;
using Chess.Core.Pieces;
using Chess.KnightsTour;
using Shouldly;

namespace Unit.Chess.KnightsTour;

public class BruteKnightsTourSolverTests
{
    [Fact]
    public void Solve_Should_Not_Take_Too_Many_Steps()
    {
        // given
        var knightPosition = new Position(0, 0);
        var board = new BoardBuilder(5, 5)
            .AddPiece(new Knight(Player.White), knightPosition)
            .Build();
        var solver = new BruteKnightsTourSolver();

        // when
        var result = solver.Solve(board);

        // then
        result.Count.ShouldBe((board.Rows * board.Columns) - 1);
    }

    [Fact]
    public void Solve_Should_Visit_All_Squares()
    {
        // given
        var knightPosition = new Position(0, 0);
        var board = new BoardBuilder(5, 5)
            .AddPiece(new Knight(Player.White), knightPosition)
            .Build();
        var solver = new BruteKnightsTourSolver();

        // when
        var result = solver.Solve(board);

        // then
        var visited = new List<Position> { knightPosition };
        visited.AddRange(result.Select(move => move.EndPosition));

        var allSquares = board.AllSquares;
        foreach (var square in allSquares)
        {
            visited.ShouldContain(square);
        }
    }

    [Fact]
    public void Solve_Should_Visit_Squares_Only_Once()
    {
        // given
        var knightPosition = new Position(0, 0);
        var board = new BoardBuilder(5, 5)
            .AddPiece(new Knight(Player.White), knightPosition)
            .Build();
        var solver = new BruteKnightsTourSolver();

        // when
        var result = solver.Solve(board);

        // then
        var visited = new List<Position> { knightPosition };
        visited.AddRange(result.Select(move => move.EndPosition));

        visited.Distinct().Count().ShouldBe(visited.Count);
    }
}
