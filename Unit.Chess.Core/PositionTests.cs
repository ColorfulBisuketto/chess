using Chess.Core;
using Shouldly;

namespace Unit.Chess.Core;

public class PositionTests
{
    [Fact]
    public void ToString_Should_Be_Correct()
    {
        // given
        var position = new Position(8, 8);

        // when
        var result = position.ToString();

        // then
        result.ShouldBe("i9");
    }

    [Fact]
    public void FromString_Should_Be_Correct()
    {
        // given
        const string stringPosition = "b8";
        var expectedPosition = new Position(7, 1);

        // when
        var result = Position.FromString(stringPosition);

        // then
        result.ShouldBe(expectedPosition);
    }

    [Theory]
    [InlineData("a1")]
    [InlineData("A1")]
    [InlineData("z8")]
    [InlineData("c20")]
    public void FromString_And_ToString_Should_Round_Trip_Lowercase(string stringPosition)
    {
        // given
        var position = stringPosition;

        // when
        var fromString = Position.FromString(position);
        var result = fromString.ToString();

        // then
        result.ShouldBe(stringPosition.ToLower());
    }
}
