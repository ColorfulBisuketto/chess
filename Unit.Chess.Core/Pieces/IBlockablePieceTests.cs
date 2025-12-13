using Chess.Core;
using Chess.Core.Pieces;
using Shouldly;

namespace Unit.Chess.Core.Pieces;

/// <summary>
/// Tests for checking if a given path is blocked by any other piece.
/// </summary>
public class BlockablePieceTests
{
    [Fact]
    public void CheckIfPathBlocked_Should_Return_False_On_An_Empty_Board()
    {
        // given
        var board = new Board(8, 8);
        var start = new Position(0, 0);
        var end = new Position(0, 7);
        IBlockablePiece piece = new Rook(Player.White);

        // when
        var result = piece.CheckIfPathBlocked(start, new RelativeMove(start, end), board);

        // then
        result.ShouldBeFalse();
    }

    [Fact]
    public void CheckIfPathBlocked_Should_Return_True_If_Path_Is_Blocked()
    {
        // given
        var board = new Board(8, 8)
            .AddPiece(new Pawn(Player.Black), new Position(0, 3));
        var start = new Position(0, 0);
        var end = new Position(0, 7);
        IBlockablePiece piece = new Rook(Player.White);

        // when
        var result = piece.CheckIfPathBlocked(start, new RelativeMove(start, end), board);

        // then
        result.ShouldBeTrue();
    }
}
