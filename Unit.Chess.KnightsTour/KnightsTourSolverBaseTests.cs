using Chess.Core;
using Chess.Core.Pieces;
using Chess.KnightsTour;
using Shouldly;

namespace Unit.Chess.KnightsTour;

public class KnightsTourSolverBaseTests
{
    [Fact]
    public void CanSolve_Should_Reject_Boards_With_To_Few_Knights()
    {
        // given
        var board = new Board(5, 5);
        var solver = new KnightsTourSolverBaseTest();

        // when
        var result = solver.CanSolve(board);

        // then
        result.ShouldBeFalse();
    }

    [Fact]
    public void CanSolve_Should_Reject_Boards_With_To_Many_Knights()
    {
        // given
        var board = new BoardBuilder(5, 5)
            .AddPiece(new Knight(Player.White), new Position(0, 0))
            .AddPiece(new Knight(Player.White), new Position(1, 1))
            .Build();
        var solver = new KnightsTourSolverBaseTest();

        // when
        var result = solver.CanSolve(board);

        // then
        result.ShouldBeFalse();
    }

    [Fact]
    public void CanSolve_Should_Reject_Boards_With_Wrong_Pieces()
    {
        // given
        var board = new BoardBuilder(5, 5)
            .AddPiece(new Pawn(Player.White), new Position(0, 0))
            .AddPiece(new Knight(Player.White), new Position(1, 1))
            .Build();
        var solver = new KnightsTourSolverBaseTest();

        // when
        var result = solver.CanSolve(board);

        // then
        result.ShouldBeFalse();
    }

    private class KnightsTourSolverBaseTest : KnightsTourSolverBase
    {
        public override List<ValidMove> Solve(Board board)
        {
            return [];
        }
    }
}
