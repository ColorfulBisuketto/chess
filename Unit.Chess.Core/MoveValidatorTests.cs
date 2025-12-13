using Chess.Core;
using Chess.Core.Pieces;
using Shouldly;

namespace Unit.Chess.Core;

public class MoveValidatorTests
{
    [Fact]
    public void ValidateCurrentMove_Should_Reject_Moves_Outside_The_Board()
    {
        // given
        var startPosition = new Position(2, 2);
        var endPosition = new Position(3, 2);
        var board = new BoardBuilder(3, 3)
            .AddPiece(new Pawn(Player.White), new Position(2, 2))
            .Build();
        var unvalidatedMove = new UnvalidatedMove("Pawn", startPosition, endPosition, string.Empty);

        // when
        var validateMove = MoveValidator.ValidateCurrentMove(unvalidatedMove, board, out _);

        // then
        validateMove.ShouldBeFalse();
    }

    [Fact]
    public void ValidateCurrentMove_Should_Allow_Castling_Without_Notation()
    {
        // given
        var kingPosition = new Position(0, 4);
        var rookPosition = new Position(0, 7);
        var kingPiece = new King(Player.White);
        var rookPiece = new Rook(Player.White);
        var unvalidatedMove = new UnvalidatedMove("King", 0, 4, 0, 6, string.Empty);
        var board = new BoardBuilder(8, 8)
            .AddPiece(kingPiece, kingPosition)
            .AddPiece(rookPiece, rookPosition)
            .Build();

        // when
        var result = MoveValidator.ValidateCurrentMove(unvalidatedMove, board, out var move);

        // then
        result.ShouldBeTrue();
        move?.SpecialPlyAction.ShouldBe(SpecialPlyAction.Castle);
    }

    [Fact]
    public void ValidateCurrentMove_Should_Allow_Castling_With_Notation()
    {
        // given
        var kingPosition = new Position(0, 4);
        var rookPosition = new Position(0, 7);
        var kingPiece = new King(Player.White);
        var rookPiece = new Rook(Player.White);
        var unvalidatedMove = new UnvalidatedMove("King", -1, -1, -1, -1, string.Empty, "O-O");
        var board = new BoardBuilder(8, 8)
            .AddPiece(kingPiece, kingPosition)
            .AddPiece(rookPiece, rookPosition)
            .Build();

        // when
        var result = MoveValidator.ValidateCurrentMove(unvalidatedMove, board, out var move);

        // then
        result.ShouldBeTrue();
        move?.SpecialPlyAction.ShouldBe(SpecialPlyAction.Castle);
    }

    [Fact]
    public void ValidateCurrentMove_Should_Allow_EnPassant_Capture()
    {
        // given
        var pawnPosition = new Position(3, 0);
        var originalPawnPosition = new Position(1, 0);
        var pawnPiece = new Pawn(Player.White);
        var otherPawnPosition = new Position(3, 1);
        var otherPawnPiece = new Pawn(Player.Black);
        var unvalidatedMove = new UnvalidatedMove("Pawn", 3, 1, 2, 0, string.Empty);
        var board = new BoardBuilder(8, 8)
            .AddPiece(pawnPiece, pawnPosition)
            .AddPiece(otherPawnPiece, otherPawnPosition)
            .Build();
        board.Turn = Player.Black;
        board.History.Push(new ValidMove(pawnPiece, originalPawnPosition, pawnPosition, null, null, null));

        // when
        var result = MoveValidator.ValidateCurrentMove(unvalidatedMove, board, out var move);

        // then
        result.ShouldBeTrue();
        move?.SpecialPlyAction.ShouldBe(SpecialPlyAction.CaptureEnPassant);
        move?.CapturedPiece.ShouldBe(pawnPiece);
    }

    [Fact]
    public void ValidateCurrentMove_Should_Only_Allow_Pawns_To_EnPassantCapture_Other_Pawns()
    {
        // given
        var pawnPosition = new Position(3, 0);
        var originalPawnPosition = new Position(1, 0);
        var pawnPiece = new Pawn(Player.White);
        var bishopPosition = new Position(3, 1);
        var bishop = new Bishop(Player.Black);
        var unvalidatedMove = new UnvalidatedMove("Bishop", 3, 1, 2, 0, string.Empty);
        var board = new BoardBuilder(8, 8)
            .AddPiece(pawnPiece, pawnPosition)
            .AddPiece(bishop, bishopPosition)
            .Build();
        board.Turn = Player.Black;
        board.History.Push(new ValidMove(pawnPiece, originalPawnPosition, pawnPosition, null, null, null));

        // when
        var result = MoveValidator.ValidateCurrentMove(unvalidatedMove, board, out var move);

        // then
        result.ShouldBeTrue();
        move?.SpecialPlyAction.ShouldBeNull();
        move?.CapturedPiece.ShouldBeNull();
    }

    [Fact]
    public void ValidateCurrentMove_Should_Return_ValidMoves()
    {
        // given
        var board = new BoardBuilder(3, 3)
            .AddPiece(new Pawn(Player.White), new Position(1, 1))
            .Build();
        var unvalidatedMove = new UnvalidatedMove("Pawn", 1, 1, 2, 1, "Queen");

        // when
        var validateMove = MoveValidator.ValidateCurrentMove(unvalidatedMove, board, out var move);

        // then
        if (validateMove)
        {
            move.ShouldBeOfType(typeof(ValidMove));
        }
    }

    [Fact]
    public void ValidateCurrentMove_Should_Promote_Pawns()
    {
        // given
        var board = new BoardBuilder(3, 3)
            .AddPiece(new Pawn(Player.White), new Position(1, 1))
            .Build();
        var unvalidatedMove = new UnvalidatedMove("Pawn", 1, 1, 2, 1, "Queen");

        // when
        var validateMove = MoveValidator.ValidateCurrentMove(unvalidatedMove, board, out var move);

        // then
        if (validateMove)
        {
            move?.PromoteToPiece.ShouldBeOfType(typeof(Queen));
        }
    }

    [Fact]
    public void CheckKingInCheck_Should_Return_True_If_Correct()
    {
        // given
        var turn = Player.Black;
        var enemyPosition = new Position(3, 0);
        var enemyPiece = new Rook(Player.White);
        var kingPosition = new Position(1, 0);
        var kingPiece = new King(Player.Black);
        var board = new BoardBuilder(4, 4)
            .AddPiece(enemyPiece, enemyPosition)
            .AddPiece(kingPiece, kingPosition)
            .Build();

        // when
        var result = MoveValidator.CheckKingInCheck(board, turn);

        // then
        result.ShouldBeTrue();
    }

    [Fact]
    public void CheckKingInCheck_Should_Return_False_If_Not()
    {
        // given
        var turn = Player.Black;
        var enemyPosition = new Position(3, 1);
        var enemyPiece = new Rook(Player.White);
        var kingPosition = new Position(1, 0);
        var kingPiece = new King(Player.Black);
        var board = new BoardBuilder(4, 4)
            .AddPiece(enemyPiece, enemyPosition)
            .AddPiece(kingPiece, kingPosition)
            .Build();

        // when
        var result = MoveValidator.CheckKingInCheck(board, turn);

        // then
        result.ShouldBeFalse();
    }

    [Fact]
    public void CheckKingInCheckMate_Should_Return_False_If_Not()
    {
        // given
        var turn = Player.Black;
        var enemyPosition = new Position(3, 0);
        var enemyPiece = new Rook(Player.White);
        var kingPosition = new Position(1, 0);
        var kingPiece = new King(Player.Black);
        var board = new BoardBuilder(4, 4)
            .AddPiece(enemyPiece, enemyPosition)
            .AddPiece(kingPiece, kingPosition)
            .Build();

        // when
        var result = MoveValidator.CheckKingInCheckMate(board, turn);

        // then
        result.ShouldBeFalse();
    }

    [Fact]
    public void CheckKingInCheckMate_Should_Return_True_If_Correct()
    {
        // given
        const Player turn = Player.Black;
        var enemyPosition1 = new Position(2, 0);
        var enemyPiece1 = new Queen(Player.White);
        var enemyPosition2 = new Position(2, 1);
        var enemyPiece2 = new Rook(Player.White);
        var kingPosition = new Position(0, 0);
        var kingPiece = new King(Player.Black);
        var board = new BoardBuilder(4, 4)
            .AddPiece(enemyPiece1, enemyPosition1)
            .AddPiece(enemyPiece2, enemyPosition2)
            .AddPiece(kingPiece, kingPosition)
            .Build();

        // when
        var result = MoveValidator.CheckKingInCheckMate(board, turn);

        // then
        result.ShouldBeTrue();
    }

    [Fact]
    public void CheckStaleMate_Should_Return_True_If_StaleMate()
    {
        // given
        var turn = Player.Black;
        var enemyPosition1 = new Position(2, 1);
        var enemyPiece1 = new Queen(Player.White);
        var kingPosition = new Position(0, 0);
        var kingPiece = new King(Player.Black);
        var board = new BoardBuilder(8, 8)
            .AddPiece(enemyPiece1, enemyPosition1)
            .AddPiece(kingPiece, kingPosition)
            .Build();

        // when
        var result = MoveValidator.CheckStaleMate(board, turn);

        // then
        result.ShouldBeTrue();
    }

    [Fact]
    public void CheckDeadPosition_Should_Return_True_If_There_Are_Only_Kings_Remaining()
    {
        // given
        var whitePosition = new Position(0, 0);
        var whiteKing = new King(Player.White);
        var blackPosition = new Position(7, 7);
        var blackKing = new King(Player.Black);
        var board = new BoardBuilder(8, 8)
            .AddPiece(whiteKing, whitePosition)
            .AddPiece(blackKing, blackPosition)
            .Build();

        // when
        var result = MoveValidator.CheckDeadPosition(board);

        // then
        result.ShouldBeTrue();
    }

    [Fact]
    public void CheckDeadPosition_Should_Return_True_If_There_Are_Only_Kings_And_One_Bishop_Remaining()
    {
        // given
        var whitePosition = new Position(0, 0);
        var whiteKing = new King(Player.White);
        var blackPosition1 = new Position(7, 7);
        var blackKing = new King(Player.Black);
        var blackPosition2 = new Position(7, 6);
        var blackBishop = new Bishop(Player.Black);
        var board = new BoardBuilder(8, 8)
            .AddPiece(whiteKing, whitePosition)
            .AddPiece(blackKing, blackPosition1)
            .AddPiece(blackBishop, blackPosition2)
            .Build();

        // when
        var result = MoveValidator.CheckDeadPosition(board);

        // then
        result.ShouldBeTrue();
    }

    [Fact]
    public void CheckDeadPosition_Should_Return_True_If_There_Are_Only_Kings_And_One_Knight_Remaining()
    {
        // given
        var whitePosition1 = new Position(0, 0);
        var whiteKing = new King(Player.White);
        var whitePosition2 = new Position(0, 1);
        var whiteKnight = new Bishop(Player.White);
        var blackPosition = new Position(7, 7);
        var blackKing = new King(Player.Black);
        var board = new BoardBuilder(8, 8)
            .AddPiece(whiteKing, whitePosition1)
            .AddPiece(whiteKnight, whitePosition2)
            .AddPiece(blackKing, blackPosition)
            .Build();

        // when
        var result = MoveValidator.CheckDeadPosition(board);

        // then
        result.ShouldBeTrue();
    }

    [Fact]
    public void CheckForThreeFoldRepetition_Should_Return_True_If_Correct()
    {
        // given
        var board = new Board(8, 8);
        var trackedBoardString = string.Join(" ", board.ToString().Split(' ').Take(4));
        board.PastBoardOccurrences[trackedBoardString] = 3;

        // when
        var result = MoveValidator.CheckForThreeFoldRepetition(board);

        // then
        result.ShouldBeTrue();
    }

    [Fact]
    public void CheckFiftyMoveRule_Should_Return_True_If_Correct()
    {
        // given
        var board = new Board(8, 8) { ReversibleMoveNumber = 51 };

        // when
        var result = MoveValidator.CheckFiftyMoveRule(board);

        // then
        result.ShouldBeTrue();
    }
}
