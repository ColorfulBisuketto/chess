using Chess.Core;
using Chess.Core.Pieces;
using Shouldly;

namespace Unit.Chess.Core.Pieces;

/// <summary>
/// Tests for the Bishop piece movement pattern.
/// </summary>
public class BishopTests
{
    [Fact]
    public void IsCorrectMovementPattern_Should_Not_Allow_Moving_Straight()
    {
        // given
        var bishop = new Bishop(Player.Black);
        var startPosition = new Position(0, 0);
        var endPosition = new Position(0, 3);

        // when
        var result = bishop.IsCorrectMovementPattern(new RelativeMove(startPosition, endPosition));

        // then
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsCorrectMovementPattern_Should_Not_Allow_Moving_Out_Of_Pattern()
    {
        // given
        var bishop = new Bishop(Player.Black);
        var startPosition = new Position(0, 0);
        var endPosition = new Position(2, 3);

        // when
        var result = bishop.IsCorrectMovementPattern(new RelativeMove(startPosition, endPosition));

        // then
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsCorrectMovementPattern_Should_Allow_Moving_Diagonally()
    {
        // given
        var bishop = new Bishop(Player.Black);
        var startPosition = new Position(0, 0);
        var endPosition = new Position(3, 3);

        // when
        var result = bishop.IsCorrectMovementPattern(new RelativeMove(startPosition, endPosition));

        // then
        result.ShouldBeTrue();
    }
}
