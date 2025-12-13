using Chess.Core;
using Chess.Core.Pieces;
using Shouldly;

namespace Unit.Chess.Core;

/// <summary>
/// Tests for the boardBuilder.
/// </summary>
public class BoardBuilderTests
{
    [Fact]
    public void EmptyStandardGame_ShouldReturn_An_8x8_Board()
    {
        // given
        // when
        var board = BoardBuilder.EmptyStandardGame();

        // then
        board.Rows.ShouldBe(8);
        board.Columns.ShouldBe(8);
        board.Squares.Length.ShouldBe(64);
    }

    [Fact]
    public void KnightsTour_ShouldReturn_An_8x8_Board_With_A_Knight_In_The_Corner()
    {
        // given
        // when
        var board = BoardBuilder.KnightsTour();

        // then
        board.Rows.ShouldBe(8);
        board.Columns.ShouldBe(8);
        board.Squares.Length.ShouldBe(64);
        board.GetPiece(new Position(0, 0)) !.Name.ShouldBe("Knight");
    }

    [Fact]
    public void StandardGame_ShouldReturn_An_8x8_Board_With_All_The_Pieces()
    {
        // given
        // when
        var board = BoardBuilder.StandardGame();

        // then
        board.Rows.ShouldBe(8);
        board.Columns.ShouldBe(8);
        board.Squares.Length.ShouldBe(64);
        board.GetPiece(new Position(0, 0)) !.Name.ShouldBe("Rook");
        board.GetPiece(new Position(0, 1)) !.Name.ShouldBe("Knight");
        board.GetPiece(new Position(0, 2)) !.Name.ShouldBe("Bishop");
        board.GetPiece(new Position(0, 3)) !.Name.ShouldBe("Queen");
        board.GetPiece(new Position(0, 4)) !.Name.ShouldBe("King");
        board.GetPiece(new Position(0, 5)) !.Name.ShouldBe("Bishop");
        board.GetPiece(new Position(0, 6)) !.Name.ShouldBe("Knight");
        board.GetPiece(new Position(0, 7)) !.Name.ShouldBe("Rook");

        for (var column = 0; column < 8; column++)
        {
            board.GetPiece(new Position(1, column)) !.Name.ShouldBe("Pawn");
            board.GetPiece(new Position(6, column)) !.Name.ShouldBe("Pawn");
        }

        for (var row = 2; row <= 5; row++)
        {
            for (var column = 0; column < 8; column++)
            {
                board.GetPiece(new Position(row, column)).ShouldBeNull();
            }
        }

        board.GetPiece(new Position(7, 0)) !.Name.ShouldBe("Rook");
        board.GetPiece(new Position(7, 1)) !.Name.ShouldBe("Knight");
        board.GetPiece(new Position(7, 2)) !.Name.ShouldBe("Bishop");
        board.GetPiece(new Position(7, 3)) !.Name.ShouldBe("Queen");
        board.GetPiece(new Position(7, 4)) !.Name.ShouldBe("King");
        board.GetPiece(new Position(7, 5)) !.Name.ShouldBe("Bishop");
        board.GetPiece(new Position(7, 6)) !.Name.ShouldBe("Knight");
        board.GetPiece(new Position(7, 7)) !.Name.ShouldBe("Rook");
        board.GetPiece(new Position(7, 0)) !.Name.ShouldBe("Rook");
    }

    [Fact]
    public void Remove_Should_Remove_A_Piece_From_The_Board()
    {
        // given
        var corner = new Position(0, 0);
        var boardBuilder = new BoardBuilder(8, 8)
            .AddPiece(new Knight(Player.Black), corner);

        // when
        var board = boardBuilder
            .RemovePiece(corner)
            .Build();

        // then
        board.GetPiece(corner).ShouldBeNull();
    }
}
