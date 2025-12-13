using Chess.Core;
using Shouldly;

namespace Unit.Chess.Core;

public class MoveParserTests
{
    [Theory]
    [InlineData("")]
    [InlineData("4")]
    [InlineData("e")]
    [InlineData("####e4")]
    [InlineData("4e")]
    [InlineData("  e4")]
    public void ParseMove_Should_Reject_Bad_Instructions(string text)
    {
        // given
        // when
        var parseable = MoveParser.TryParseMove(text, out _);

        // then
        parseable.ShouldBeFalse();
    }

    [Theory]
    [InlineData("e4")]
    [InlineData("aa8")]
    [InlineData("7c3")]
    [InlineData("Pa8=Q")]
    [InlineData("Paa8Q")]
    public void ParseMove_Should_Accept_Valid_Instructions(string text)
    {
        // given
        // when
        var parseable = MoveParser.TryParseMove(text, out _);

        // then
        parseable.ShouldBeTrue();
    }

    [Fact]
    public void ParseMove_Should_Return_UnvalidatedMoves()
    {
        // given
        const string text = "aa8=Q";

        // when
        _ = MoveParser.TryParseMove(text, out var move);

        // then
        move.ShouldBeOfType(typeof(UnvalidatedMove));
    }

    [Fact]
    public void ParseMove_Should_Allow_Castling_Notation()
    {
        // given
        const string text = "O-O-O";

        // when
        _ = MoveParser.TryParseMove(text, out var move);

        // then
        move.ShouldBeOfType(typeof(UnvalidatedMove));
    }
}
