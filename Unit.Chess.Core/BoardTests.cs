using Chess.Core;
using Chess.Core.Pieces;
using Shouldly;

namespace Unit.Chess.Core;

public class BoardTests
{
    [Fact]
    public void AllSquares_Should_Contain_All_Squares()
    {
        // given
        var dimensions = (3, 3);

        // when
        var board = new Board(dimensions.Item1, dimensions.Item2);

        // then
        for (var row = 0; row < dimensions.Item1; row++)
        {
            for (var column = 0; column < dimensions.Item2; column++)
            {
                board.AllSquares.ShouldContain(new Position(row, column));
            }
        }
    }

    [Fact]
    public void AllSquares_Should_Not_Contain_Invalid_Squares()
    {
        // given
        // when
        var dimensions = (3, 3);
        var board = new Board(dimensions.Item1, dimensions.Item2);

        // then
        foreach (var square in board.AllSquares)
        {
            board.IsValidPosition(square).ShouldBeTrue();
        }
    }

    [Fact]
    public void GetPiecesByName_Should_Only_Return_Pieces_With_Correct_Name()
    {
        // given
        var dimensions = (3, 3);
        var piece1Position = new Position(0, 2);
        var piece2Position = new Position(1, 1);
        var board = new Board(dimensions.Item1, dimensions.Item2);
        board.SetPiece(new Pawn(Player.White), piece1Position);
        board.SetPiece(new Rook(Player.White), piece2Position);

        // when
        var result = board.GetPiecesByName("Pawn", Player.White);

        // then
        result.Count.ShouldBe(1);
        result.ShouldContain(piece1Position);
        result.ShouldNotContain(piece2Position);
    }

    [Fact]
    public void GetPiecesByName_Should_Not_Return_Enemy_Positions()
    {
        // given
        var dimensions = (3, 3);
        var piece1Position = new Position(0, 2);
        var piece2Position = new Position(1, 1);
        var board = new Board(dimensions.Item1, dimensions.Item2);
        board.SetPiece(new Pawn(Player.White), piece1Position);
        board.SetPiece(new Pawn(Player.Black), piece2Position);

        // when
        var result = board.GetPiecesByName("Pawn", Player.White);

        // then
        result.Count.ShouldBe(1);
        result.ShouldContain(piece1Position);
        result.ShouldNotContain(piece2Position);
    }

    [Fact]
    public void GetPiecesByPlayer_Should_Not_Return_Enemy_Positions()
    {
        // given
        var dimensions = (3, 3);
        var piece1Position = new Position(0, 2);
        var piece2Position = new Position(1, 1);
        var board = new Board(dimensions.Item1, dimensions.Item2);
        board.SetPiece(new Pawn(Player.White), piece1Position);
        board.SetPiece(new Rook(Player.Black), piece2Position);

        // when
        var result = board.GetPiecesByPlayer(Player.White);

        // then
        result.Count.ShouldBe(1);
        result.ShouldContain(piece1Position);
        result.ShouldNotContain(piece2Position);
    }

    [Fact]
    public void ToString_Should_Be_Accurate()
    {
        // given
        // Example Game Starting Position from https://en.wikipedia.org/wiki/Forsyth%E2%80%93Edwards_Notation
        const string expected = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        var board = BoardBuilder.StandardGame();

        // when
        var result = board.ToString();

        // then
        result.ShouldBe(expected);
    }

    [Fact]
    public void GetPossibleEnPassantCapturePosition_Should_Output_The_Correct_Position()
    {
        // given
        var startPosition = new Position(0, 0);
        var skippedPosition = new Position(1, 0);
        var endPosition = new Position(2, 0);
        var pawn = new Pawn(Player.White);
        var board = new Board(8, 8);
        var validMove = new ValidMove(pawn, startPosition, endPosition, null, null, null);
        board.History.Push(validMove);

        // when
        var result = board.GetPossibleEnPassantCapturePosition();

        // then
        result.ShouldBe(skippedPosition);
    }

    [Fact]
    public void GetPossibleEnPassantCapturePosition_Should_Not_Invent_Moves()
    {
        // given
        var board = new Board(8, 8);

        // when
        var result = board.GetPossibleEnPassantCapturePosition();

        // then
        result.ShouldBeNull();
    }

    [Fact]
    public void GetPossibleEnPassantCapturePosition_Should_Not_Accuse_Pawns()
    {
        // given
        var startPosition = new Position(0, 0);
        var endPosition = new Position(1, 0);
        var pawn = new Pawn(Player.White);
        var board = new Board(8, 8);
        var validMove = new ValidMove(pawn, startPosition, endPosition, null, null, null);
        board.History.Push(validMove);

        // when
        var result = board.GetPossibleEnPassantCapturePosition();

        // then
        result.ShouldBeNull();
    }
}
