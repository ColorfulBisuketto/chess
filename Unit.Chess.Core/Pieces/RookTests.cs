using Chess.Core;
using Chess.Core.Pieces;
using Shouldly;

namespace Unit.Chess.Core.Pieces;

/// <summary>
/// Tests for the Rook piece movement pattern.
/// </summary>
public class RookTests
{
    [Fact]
    public void IsCorrectMovementPattern_Should_Not_Allow_Moving_Diagonally()
    {
        // given
        var rook = new Rook(Player.Black);
        var startPosition = new Position(0, 0);
        var endPosition = new Position(3, 3);

        // when
        var result = rook.IsCorrectMovementPattern(new RelativeMove(startPosition, endPosition));

        // then
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsCorrectMovementPattern_Should_Not_Allow_Moving_Out_Of_Pattern()
    {
        // given
        var rook = new Rook(Player.Black);
        var startPosition = new Position(0, 0);
        var endPosition = new Position(2, 3);

        // when
        var result = rook.IsCorrectMovementPattern(new RelativeMove(startPosition, endPosition));

        // then
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsCorrectMovementPattern_Should_Allow_Moving_Straight()
    {
        // given
        var rook = new Rook(Player.Black);
        var startPosition = new Position(0, 0);
        var endPosition = new Position(3, 0);

        // when
        var result = rook.IsCorrectMovementPattern(new RelativeMove(startPosition, endPosition));

        // then
        result.ShouldBeTrue();
    }
}
