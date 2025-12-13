using Chess.Core;
using Chess.Core.Pieces;
using Shouldly;

namespace Unit.Chess.Core.Pieces;

/// <summary>
/// Tests for the Knight piece movement pattern.
/// </summary>
public class KnightTests
{
    [Fact]
    public void IsCorrectMovementPattern_Should_Not_Allow_Moving_Straight()
    {
        // given
        var knight = new Knight(Player.Black);
        var startPosition = new Position(0, 0);
        var endPosition = new Position(0, 3);

        // when
        var result = knight.IsCorrectMovementPattern(new RelativeMove(startPosition, endPosition));

        // then
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsCorrectMovementPattern_Should_Not_Allow_Moving_Out_Of_Pattern()
    {
        // given
        var knight = new Knight(Player.Black);
        var startPosition = new Position(0, 0);
        var endPosition = new Position(3, 3);

        // when
        var result = knight.IsCorrectMovementPattern(new RelativeMove(startPosition, endPosition));

        // then
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsCorrectMovementPattern_Should_Allow_Moving_Correctly()
    {
        // given
        var knight = new Knight(Player.Black);
        var startPosition = new Position(0, 0);
        var endPosition = new Position(1, 2);

        // when
        var result = knight.IsCorrectMovementPattern(new RelativeMove(startPosition, endPosition));

        // then
        result.ShouldBeTrue();
    }
}
