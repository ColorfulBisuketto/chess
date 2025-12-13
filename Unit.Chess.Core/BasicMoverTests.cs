using Chess.Core;
using Chess.Core.Pieces;
using Shouldly;

namespace Unit.Chess.Core;

/// <summary>
/// Tests for the basic mover.
/// </summary>
public class BasicMoverTests
{
    [Fact]
    public void GetPossibleMoves_Should_Find_Allowed_Moves()
    {
        // given
        var pawnPosition = new Position(1, 3);
        var pawnPiece = new Pawn(Player.White);
        var board = new BoardBuilder(8, 8)
            .AddPiece(pawnPiece, pawnPosition)
            .Build();
        var mover = new BasicMover(board);

        // when
        var possibleMoves = mover.GetPossibleMoves(pawnPosition, Player.White);

        // then
        possibleMoves.Count.ShouldBe(2);
        possibleMoves.ShouldContain(new ValidMove(pawnPiece, pawnPosition, new Position(2, 3), null, null, null));
        possibleMoves.ShouldContain(new ValidMove(pawnPiece, pawnPosition, new Position(3, 3), null, null, null));
    }

    [Fact]
    public void GetPossibleMoves_Should_Not_Find_Moves_Outside_The_Board()
    {
        // given
        var knightPosition = new Position(7, 7);
        var board = new BoardBuilder(8, 8)
            .AddPiece(new Knight(Player.Black), knightPosition)
            .Build();
        var mover = new BasicMover(board);

        // when
        var possibleMoves = mover.GetPossibleMoves(knightPosition, Player.Black);

        // then
        foreach (var move in possibleMoves)
        {
            var res = move.EndPosition is { Row: >= 0 and < 8, Column: >= 0 and < 8 };
            res.ShouldBeTrue();
        }
    }

    [Fact]
    public void GetPossibleMoves_Should_Not_Find_Valid_Moves_For_Empty_Squares()
    {
        // given
        var blankPosition = new Position(3, 3);
        var board = new Board(8, 8);
        var mover = new BasicMover(board);

        // when
        var possibleMoves = mover.GetPossibleMoves(blankPosition, Player.White);

        // then
        possibleMoves.ShouldBeEmpty();
    }

    [Fact]
    public void GetPossible_Moves_Should_Not_Find_Blocked_Moves()
    {
        // given
        var rookPosition = new Position(0, 3);
        var rookPiece = new Rook(Player.White);
        var pawnPosition = new Position(0, 2);
        var pawnPiece = new Pawn(Player.Black);
        var board = new BoardBuilder(8, 8)
            .AddPiece(rookPiece, rookPosition)
            .AddPiece(pawnPiece, pawnPosition)
            .Build();
        var mover = new BasicMover(board);

        // when
        var moves = mover.GetPossibleMoves(rookPosition, Player.White);

        // then
        moves.ShouldNotContain(move => move.EndPosition.Column < 2);
    }

    [Fact]
    public void MakeMove_Should_Increment_Times_Moved_Of_The_Piece()
    {
        // given
        var knightPosition = new Position(0, 0);
        var newPosition = new Position(2, 1);
        var knightPiece = new Knight(Player.Black);
        var move = new ValidMove(knightPiece, knightPosition, newPosition, null, null, null);
        var board = new BoardBuilder(8, 8)
            .AddPiece(knightPiece, knightPosition)
            .Build();
        var mover = new BasicMover(board);

        // when
        mover.Move(move);

        // then
        knightPiece.TimesMoved.ShouldBe(1);
    }

    // Castling
    [Fact]
    public void MakeMove_Should_Properly_Castle()
    {
        // given
        var kingPosition = new Position(0, 4);
        var rookPosition = new Position(0, 7);
        var newPosition = new Position(0, 6);
        var newRookPosition = new Position(0, 5);
        var kingPiece = new King(Player.White);
        var rookPiece = new Rook(Player.White);
        var move = new ValidMove(kingPiece, kingPosition, newPosition, null, SpecialPlyAction.Castle, null);
        var board = new BoardBuilder(8, 8)
            .AddPiece(kingPiece, kingPosition)
            .AddPiece(rookPiece, rookPosition)
            .Build();
        var mover = new BasicMover(board);

        // when
        mover.Move(move);

        // then
        kingPiece.TimesMoved.ShouldBe(1);
        board.GetPiece(newPosition)?.Name.ShouldBe("King");
        board.GetPiece(kingPosition).ShouldBeNull();
        rookPiece.TimesMoved.ShouldBe(1);
        board.GetPiece(newRookPosition)?.Name.ShouldBe("Rook");
        board.GetPiece(rookPosition).ShouldBeNull();
    }

    // Promotion
    [Fact]
    public void MakeMove_Should_Properly_Promote()
    {
        // given
        var pawnPosition = new Position(6, 0);
        var newPosition = new Position(7, 0);
        var pawn = new Pawn(Player.White);
        var promotionPiece = new Queen(Player.White);
        var move = new ValidMove(pawn, pawnPosition, newPosition, null, SpecialPlyAction.Promote, promotionPiece);
        var board = new BoardBuilder(8, 8)
            .AddPiece(pawn, pawnPosition)
            .Build();
        var mover = new BasicMover(board);

        // when
        mover.Move(move);

        // then
        board.GetPiece(newPosition)?.Name.ShouldBe("Queen");
        board.GetPiece(newPosition)?.TimesMoved.ShouldBe(1);
        board.GetPiece(pawnPosition).ShouldBeNull();
    }

    [Fact]
    public void MakeMove_Should_Correctly_EnPassant_Capture()
    {
        // given
        var pawnPosition = new Position(3, 0);
        var skippedPawnPosition = new Position(2, 0);
        var originalPawnPosition = new Position(1, 0);
        var pawn = new Pawn(Player.White) { TimesMoved = 1 };
        var otherPawnPosition = new Position(3, 1);
        var otherPawn = new Pawn(Player.Black);
        var move = new ValidMove(
            otherPawn,
            otherPawnPosition,
            skippedPawnPosition,
            pawn,
            SpecialPlyAction.CaptureEnPassant,
            null);
        var board = new BoardBuilder(8, 8)
            .AddPiece(pawn, pawnPosition)
            .AddPiece(otherPawn, otherPawnPosition)
            .Build();
        board.History.Push(new ValidMove(pawn, originalPawnPosition, pawnPosition, null, null, null));
        var mover = new BasicMover(board);

        // when
        mover.Move(move);

        // then
        board.GetPiece(pawnPosition).ShouldBeNull();
        board.History.Peek().CapturedPiece.ShouldBe(pawn);
    }

    [Fact]
    public void Undo_Should_Restore_MoveHistory()
    {
        // given
        var knightPosition = new Position(0, 0);
        var knight = new Knight(Player.Black);
        var newPosition = new Position(2, 1);
        var board = new BoardBuilder(3, 3)
            .AddPiece(knight, newPosition)
            .Build();
        var mover = new BasicMover(board);
        board.History.Push(new ValidMove(knight, knightPosition, newPosition, null, null, null));

        // when
        mover.Undo();

        // then
        board.History.Count.ShouldBe(0);
    }

    [Fact]
    public void Undo_Should_Restore_Positions()
    {
        // given
        var knightPosition = new Position(0, 0);
        var knight = new Knight(Player.Black);
        var newPosition = new Position(2, 1);
        var board = new BoardBuilder(3, 3)
            .AddPiece(knight, newPosition)
            .Build();
        var mover = new BasicMover(board);
        board.History.Push(new ValidMove(knight, knightPosition, newPosition, null, null, null));

        // when
        mover.Undo();

        // then
        board.GetPiece(newPosition).ShouldBeNull();
        board.GetPiece(knightPosition) !.Name.ShouldBe("Knight");
    }

    [Fact]
    public void Undo_Should_Restore_Pieces()
    {
        // given
        var knightPosition = new Position(0, 0);
        var knight = new Knight(Player.Black);
        var pawnPosition = new Position(2, 1);
        var board = new BoardBuilder(3, 3)
            .AddPiece(new Knight(Player.Black), pawnPosition)
            .Build();
        var mover = new BasicMover(board);
        board.History.Push(new ValidMove(knight, knightPosition, pawnPosition, new Pawn(Player.White), null, null));

        // when
        mover.Undo();

        // then
        board.GetPiece(pawnPosition) !.Name.ShouldBe("Pawn");
        board.GetPiece(knightPosition) !.Name.ShouldBe("Knight");
    }

    [Fact]
    public void Undo_Should_Restore_Castling()
    {
        // given
        var kingEndPosition = new Position(0, 6);
        var kingStartPosition = new Position(0, 4);
        var rookEndPosition = new Position(0, 5);
        var rookStartPosition = new Position(0, 7);
        var king = new King(Player.White) { TimesMoved = 1 };
        var rook = new Rook(Player.White) { TimesMoved = 1 };
        var board = new BoardBuilder(8, 8)
            .AddPiece(rook, rookEndPosition)
            .AddPiece(king, kingEndPosition)
            .Build();
        var lastMove = new ValidMove(king, kingStartPosition, kingEndPosition, null, SpecialPlyAction.Castle, null);
        board.History.Push(lastMove);
        var mover = new BasicMover(board);

        // when
        mover.Undo();

        // then
        board.GetPiece(kingEndPosition).ShouldBeNull();
        board.GetPiece(kingStartPosition) !.Name.ShouldBe("King");
        board.GetPiece(rookEndPosition).ShouldBeNull();
        board.GetPiece(rookStartPosition) !.Name.ShouldBe("Rook");
    }

    [Fact]
    public void Undo_Should_Restore_EnPassant_Capture()
    {
        // given
        var pawnPosition = new Position(3, 0);
        var skippedPawnPosition = new Position(2, 0);
        var originalPawnPosition = new Position(1, 0);
        var pawn = new Pawn(Player.White);
        var otherPawnPosition = new Position(3, 1);
        var otherPawn = new Pawn(Player.Black);
        var board = new BoardBuilder(8, 8)
            .AddPiece(otherPawn, skippedPawnPosition)
            .Build();
        board.History.Push(new ValidMove(pawn, originalPawnPosition, pawnPosition, null, null, null));
        board.History.Push(
            new ValidMove(
                otherPawn,
                otherPawnPosition,
                skippedPawnPosition,
                pawn,
                SpecialPlyAction.CaptureEnPassant,
                null));
        var mover = new BasicMover(board);

        // when
        mover.Undo();

        // then
        board.GetPiece(pawnPosition) !.Name.ShouldBe("Pawn");
        board.GetPiece(otherPawnPosition) !.Name.ShouldBe("Pawn");
        board.GetPiece(skippedPawnPosition).ShouldBeNull();
    }
}
