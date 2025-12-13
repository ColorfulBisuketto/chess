using Chess.Core;
using Chess.Core.Pieces;
using Shouldly;

namespace Unit.Chess.Core.Pieces;

/// <summary>
/// Tests for the King piece movement pattern and Special Rules.
/// </summary>
public class KingTests
{
    [Fact]
    public void IsCorrectMovementPattern_Should_Not_Allow_Moving_Too_Far()
    {
        // given
        var king = new King(Player.Black);
        var startPosition = new Position(0, 0);
        var endPosition = new Position(2, 0);

        // when
        var result = king.IsCorrectMovementPattern(new RelativeMove(startPosition, endPosition));

        // then
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsCorrectMovementPattern_Should_Allow_Moving_Correctly()
    {
        // given
        var king = new King(Player.Black);
        var startPosition = new Position(0, 0);
        var endPosition = new Position(1, 1);

        // when
        var result = king.IsCorrectMovementPattern(new RelativeMove(startPosition, endPosition));

        // then
        result.ShouldBeTrue();
    }

    [Fact]
    public void SpecialRules_Should_Detect_Castling()
    {
        // given
        var kingPosition = new Position(0, 4);
        var rookPosition = new Position(0, 7);
        var newPosition = new Position(0, 6);
        var kingPiece = new King(Player.White);
        var rookPiece = new Rook(Player.White);
        var board = new Board(8, 8)
            .AddPiece(kingPiece, kingPosition)
            .AddPiece(rookPiece, rookPosition);

        // when
        _ = kingPiece.SpecialRule(kingPosition, newPosition, board, out var action);

        // then
        action.ShouldBe(SpecialPlyAction.Castle);
    }

    [Fact]
    public void SpecialRules_Should_Not_Allow_Castling_If_King_has_Moved()
    {
        // given
        var kingPosition = new Position(0, 4);
        var rookPosition = new Position(0, 7);
        var newPosition = new Position(0, 6);
        var kingPiece = new King(Player.White) { TimesMoved = 2 };
        var rookPiece = new Rook(Player.White);
        var board = new Board(8, 8)
            .AddPiece(kingPiece, kingPosition)
            .AddPiece(rookPiece, rookPosition);

        // when
        var result = kingPiece.SpecialRule(kingPosition, newPosition, board, out _);

        // then
        result.ShouldBeFalse();
    }

    [Fact]
    public void SpecialRules_Should_Not_Allow_Castling_If_Rook_has_Moved()
    {
        // given
        var kingPosition = new Position(0, 4);
        var rookPosition = new Position(0, 0);
        var newPosition = new Position(0, 2);
        var kingPiece = new King(Player.White);
        var rookPiece = new Rook(Player.White) { TimesMoved = 2 };
        var board = new Board(8, 8)
            .AddPiece(kingPiece, kingPosition)
            .AddPiece(rookPiece, rookPosition);

        // when
        var result = kingPiece.SpecialRule(kingPosition, newPosition, board, out _);

        // then
        result.ShouldBeFalse();
    }

    [Fact]
    public void SpecialRules_Should_Not_Allow_Castling_If_Rook_Doesnt_Exist()
    {
        // given
        var kingPosition = new Position(0, 4);
        var newPosition = new Position(0, 2);
        var kingPiece = new King(Player.White);
        var board = new Board(8, 8)
            .AddPiece(kingPiece, kingPosition);

        // when
        var result = kingPiece.SpecialRule(kingPosition, newPosition, board, out _);

        // then
        result.ShouldBeFalse();
    }
}
