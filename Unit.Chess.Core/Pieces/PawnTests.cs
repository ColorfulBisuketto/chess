using Chess.Core;
using Chess.Core.Pieces;
using Shouldly;

namespace Unit.Chess.Core.Pieces;

/// <summary>
/// Tests for the Pawn piece movement pattern and Special Rules.
/// </summary>
public class PawnTests
{
    [Fact]
    public void IsCorrectMovementPattern_Should_Not_Allow_Moves_To_Far()
    {
        // given
        var pawn = new Pawn(Player.White);
        var startPosition = new Position(0, 0);
        var endPosition = new Position(3, 0);

        // when
        var result = pawn.IsCorrectMovementPattern(new RelativeMove(startPosition, endPosition));

        // then
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsCorrectMovementPattern_Should_Not_Allow_Sideway_Moves()
    {
        // given
        var pawn = new Pawn(Player.White);
        var startPosition = new Position(0, 0);
        var endPosition = new Position(0, 1);

        // when
        var result = pawn.IsCorrectMovementPattern(new RelativeMove(startPosition, endPosition));

        // then
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsCorrectMovementPattern_Should_Allow_Correct_Moves()
    {
        // given
        var pawn = new Pawn(Player.White);
        var startPosition = new Position(0, 0);
        var endPosition = new Position(1, 0);

        // when
        var result = pawn.IsCorrectMovementPattern(new RelativeMove(startPosition, endPosition));

        // then
        result.ShouldBeTrue();
    }

    [Fact]
    public void SpecialRules_Should_Allow_En_Passant_If_The_Piece_Has_Not_Moved()
    {
        // given
        var pawn = new Pawn(Player.White);
        var board = new Board(5, 5);
        var startPosition = new Position(0, 0);
        var endPosition = new Position(2, 0);

        // when
        var result = pawn.SpecialRule(startPosition, endPosition, board, out _);

        // then
        result.ShouldBeTrue();
    }

    [Fact]
    public void SpecialRules_Should_Not_En_Passant_If_The_Piece_Has_Moved()
    {
        // given
        var pawn = new Pawn(Player.White) { TimesMoved = 1 };
        var board = new Board(5, 5);
        var startPosition = new Position(0, 0);
        var endPosition = new Position(2, 0);

        // when
        var result = pawn.SpecialRule(startPosition, endPosition, board, out _);

        // then
        result.ShouldBeFalse();
    }

    [Fact]
    public void SpecialRules_Should_Not_Allow_Moving_Like_A_Knight()
    {
        // given
        var pawn = new Pawn(Player.White);
        var board = new Board(5, 5);
        var startPosition = new Position(0, 0);
        var endPosition = new Position(2, 1);

        // when
        var result = pawn.SpecialRule(startPosition, endPosition, board, out _);

        // then
        result.ShouldBeFalse();
    }

    [Fact]
    public void SpecialRules_Should_Not_Allow_Moves_Backwards()
    {
        // given
        var pawn = new Pawn(Player.White);
        var board = new Board(5, 5);
        var startPosition = new Position(2, 0);
        var endPosition = new Position(1, 0);

        // when
        var result = pawn.SpecialRule(startPosition, endPosition, board, out _);

        // then
        result.ShouldBeFalse();
    }

    [Fact]
    public void SpecialRules_Should_Allow_Diagonal_Captures()
    {
        // given
        var pawn = new Pawn(Player.White);
        var board = new Board(5, 5);
        board.SetPiece(new Pawn(Player.Black), new Position(1, 1));
        var startPosition = new Position(0, 0);
        var endPosition = new Position(1, 1);

        // when
        var result = pawn.SpecialRule(startPosition, endPosition, board, out _);

        // then
        result.ShouldBeTrue();
    }

    [Fact]
    public void SpecialRules_Should_Not_Allow_Straight_Captures()
    {
        // given
        var pawn = new Pawn(Player.White);
        var board = new Board(5, 5);
        board.SetPiece(new Pawn(Player.Black), new Position(1, 0));
        var startPosition = new Position(0, 0);
        var endPosition = new Position(1, 0);

        // when
        var result = pawn.SpecialRule(startPosition, endPosition, board, out _);

        // then
        result.ShouldBeFalse();
    }

    [Fact]
    public void SpecialRules_Should_Detect_Promotion()
    {
        // given
        var pawn = new Pawn(Player.White);
        var board = new Board(3, 3);
        var startPosition = new Position(0, 0);
        var endPosition = new Position(2, 0);

        // when
        pawn.SpecialRule(startPosition, endPosition, board, out var action);

        // then
        action.ShouldBe(SpecialPlyAction.Promote);
    }

    [Fact]
    public void SpecialRules_Should_Detect_EnPassant_Capture()
    {
        // given
        var pawn = new Pawn(Player.White);
        var otherPawn = new Pawn(Player.Black);
        var startPosition = new Position(4, 1);
        var otherStartPosition = new Position(6, 0);
        var otherSkippedPosition = new Position(5, 0);
        var otherEndPosition = new Position(4, 0);
        var lastMove = new ValidMove(otherPawn, otherStartPosition, otherEndPosition, null, null, null);
        var board = new Board(8, 8);
        board.History.Push(lastMove);

        // when
        pawn.SpecialRule(startPosition, otherSkippedPosition, board, out var action);

        // then
        action.ShouldBe(SpecialPlyAction.CaptureEnPassant);
    }
}
