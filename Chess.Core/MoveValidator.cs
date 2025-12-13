using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Chess.Core.Pieces;

namespace Chess.Core;

/// <summary>
/// Handles all move validation logic.
/// </summary>
public static partial class MoveValidator
{
    /// <summary>
    /// Try to validate a move on the current turn.
    /// </summary>
    /// <param name="move">The unvalidated move to validate.</param>
    /// <param name="board">The board the move is being made on.</param>
    /// <param name="validMove">Returns the validMove. Null if not valid.</param>
    /// <returns>True if the move is valid.</returns>
    public static bool ValidateCurrentMove(
        UnvalidatedMove move, Board board, [NotNullWhen(true)] out ValidMove? validMove)
    {
        var isValid = ValidatePossibleMove(move, board, board.Turn, out validMove);
        if (validMove is not { SpecialPlyAction: SpecialPlyAction.Promote } || validMove.PromoteToPiece != null)
        {
            return isValid;
        }

        validMove = null;
        return false;
    }

    /// <summary>
    /// Check if a given players king is in check.
    /// </summary>
    /// <param name="board">The board to check.</param>
    /// <param name="turn">The player whose king to check.</param>
    /// <returns>True if the king is threatened by enemy pieces.</returns>
    public static bool CheckKingInCheck(Board board, Player turn)
    {
        var kingPositions = board.GetPiecesByName("King", turn);
        if (kingPositions.Count != 1)
        {
            return false;
        }

        var kingPosition = kingPositions.First();

        var enemyPlayer = turn == Player.White ? Player.Black : Player.White;
        var enemyPositions = board.GetPiecesByPlayer(enemyPlayer);
        foreach (var enemyPosition in enemyPositions)
        {
            var piece = board.GetPiece(enemyPosition);
            if (piece != null)
            {
                var possibleMove = new UnvalidatedMove(
                    piece.Name,
                    enemyPosition.Row,
                    enemyPosition.Column,
                    kingPosition.Row,
                    kingPosition.Column,
                    "Queen");
                var isValid = ValidatePossibleMove(possibleMove, board, enemyPlayer, out _);
                if (isValid)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Check if a given players king is in checkmate.
    /// </summary>
    /// <param name="board">The board to check.</param>
    /// <param name="turn">The player whose king to check.</param>
    /// <returns>True if the king is threatened by enemy pieces and there are no legal ways of escaping.</returns>
    public static bool CheckKingInCheckMate(Board board, Player turn)
    {
        return CheckKingInCheck(board, turn) && !CanEscapeCheck(board, turn);
    }

    /// <summary>
    /// Check if a given player is in stalemate.
    /// </summary>
    /// <param name="board">The board to check.</param>
    /// <param name="turn">The player to check.</param>
    /// <returns>True if the king is not by enemy pieces but there are no ways legal moves left.</returns>
    public static bool CheckStaleMate(Board board, Player turn)
    {
        return !CheckKingInCheck(board, turn) && !CanEscapeCheck(board, turn);
    }

    /// <summary>
    /// Check if the current position is dead.
    ///
    /// <para>
    /// Uses a ver simple approach that does not find most dead positions but the most simple ones:
    /// <list type="bullet">
    ///     <item> King against King </item>
    ///     <item> King against King and Bishop </item>
    ///     <item> King against King and Knight </item>
    /// </list>
    /// </para>
    /// <see href="https://en.wikipedia.org/wiki/Rules_of_chess#Dead_position"/>
    /// </summary>
    /// <param name="board">The board to check.</param>
    /// <returns>True if there is no sequence of legal moves that allow for a checkmate by either player.</returns>
    public static bool CheckDeadPosition(Board board)
    {
        var playerOnePositions = board.GetPiecesByPlayer(Player.White);
        var playerTwoPosition = board.GetPiecesByPlayer(Player.Black);

        if ((playerOnePositions.Count != 2 || playerTwoPosition.Count != 1) &&
            (playerOnePositions.Count != 1 || playerTwoPosition.Count != 2) &&
            (playerOnePositions.Count != 1 || playerTwoPosition.Count != 1))
        {
            return false;
        }

        if (playerOnePositions.Count > playerTwoPosition.Count)
        {
            (playerOnePositions, playerTwoPosition) = (playerTwoPosition, playerOnePositions);
        }

        var playerOnePosition = playerOnePositions.First();
        var playerOnePiece = board.GetPiece(playerOnePosition);
        if (playerOnePiece is not King)
        {
            return false;
        }

        if (playerTwoPosition
            .Select(board.GetPiece)
            .Any(playerTwoPiece =>
                playerTwoPiece is not King && playerTwoPiece is not Bishop && playerTwoPiece is not Knight))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Check if this same board state has occured at least three times.
    /// </summary>
    /// <param name="board">The board to check.</param>
    /// <returns>True if this board state has occured at least three times.</returns>
    public static bool CheckForThreeFoldRepetition(Board board)
    {
        var trackedBoardString = string.Join(" ", board.ToString().Split(' ').Take(4));
        return board.PastBoardOccurrences[trackedBoardString] >= 3;
    }

    /// <summary>
    /// Check if there have been more than 50 reversible moves made.
    /// </summary>
    /// <param name="board">The board to check.</param>
    /// <returns>True if there have been more than 50 reversible moves.</returns>
    public static bool CheckFiftyMoveRule(Board board)
    {
        return board.ReversibleMoveNumber >= 50;
    }

    /// <summary>
    /// Try to validate a move as if the current turn were the given <paramref name="turn"/>.
    /// </summary>
    /// <param name="unvalidatedMove">The unvalidated move to validate.</param>
    /// <param name="board">The board the move is being made on.</param>
    /// <param name="turn">The turn that should be used to validate the move.</param>
    /// <param name="validMove">Returns the validMove. Null if not valid.</param>
    /// <returns>True if the move is valid.</returns>
    internal static bool ValidatePossibleMove(
        UnvalidatedMove unvalidatedMove, Board board, Player? turn, [NotNullWhen(true)] out ValidMove? validMove)
    {
        validMove = null;

        // Handle Castling Notation
        UnvalidatedMove move;
        if (unvalidatedMove.CastleString != string.Empty)
        {
            var queenSideCastle = QueenSideCastle();
            var kingStartPosition = board.GetPiecesByName("King", board.Turn).First();
            Position kingEndPosition;
            if (queenSideCastle.IsMatch(unvalidatedMove.CastleString))
            {
                kingEndPosition = kingStartPosition with { Column = kingStartPosition.Column - 2 };
            }
            else
            {
                kingEndPosition = kingStartPosition with { Column = kingStartPosition.Column + 2 };
            }

            move = unvalidatedMove with { StartColumn = kingStartPosition.Column, StartRow = kingStartPosition.Row, EndColumn = kingEndPosition.Column, EndRow = kingEndPosition.Row };
        }
        else
        {
            move = unvalidatedMove;
        }

        // End Position
        if (move.EndRow < 0 || move.EndColumn < 0)
        {
            return false;
        }

        var endPosition = new Position(move.EndRow, move.EndColumn);
        if (!board.IsValidPosition(endPosition))
        {
            return false;
        }

        var capturedPiece = board.GetPiece(endPosition);
        if (turn != null && capturedPiece != null && capturedPiece.Player == turn)
        {
            return false;
        }

        // Determine Start Position & Action
        var startPositionsPairs = GetStartPositionsWithSpecialAction(move, board, turn, endPosition);
        if (startPositionsPairs.Count != 1)
        {
            return false; // Start Position not found/ ambiguous
        }

        var (startPosition, specialAction) = startPositionsPairs.First();

        var piece = board.GetPiece(startPosition) !;
        if (turn != null && piece.Player != turn)
        {
            return false;
        }

        // EnPassant Capture
        if (specialAction == SpecialPlyAction.CaptureEnPassant)
        {
            var hasHistory = board.History.TryPeek(out var lastMove);
            if (hasHistory && lastMove is not null)
            {
                capturedPiece = lastMove.Piece;
            }
        }

        // Pawn Promotion
        Piece? promoteToPiece = null;
        if (specialAction == SpecialPlyAction.Promote)
        {
            promoteToPiece = move.PromotionName switch
            {
                "Queen" => new Queen(piece.Player, piece.TimesMoved),
                "Rook" => new Rook(piece.Player, piece.TimesMoved),
                "Knight" => new Knight(piece.Player, piece.TimesMoved),
                "Bishop" => new Bishop(piece.Player, piece.TimesMoved),
                _ => null,
            };
        }

        validMove = new ValidMove(piece, startPosition, endPosition, capturedPiece, specialAction, promoteToPiece);

        // If the enemy didn't escape from check allow for capturing even if threatened yourself.
        // This ensures that checkmate cant be escaped from if you threaten the enemy King.
        if (capturedPiece is not King)
        {
            // would the king be threatened?
            var mover = new BasicMover(board);
            mover.Move(validMove);
            var threatened = turn != null && CheckKingInCheck(board, (Player)turn);
            mover.Undo();
            if (threatened)
            {
                validMove = null;
                return false;
            }
        }

        return true;
    }

    private static Dictionary<Position, SpecialPlyAction?> GetStartPositionsWithSpecialAction(
        UnvalidatedMove move, Board board, Player? turn, Position endPosition)
    {
        var possibleStartPositions = InferPositions(move, board, turn);
        var startPositionsPairs = new Dictionary<Position, SpecialPlyAction?>();

        foreach (var possibleStartPosition in possibleStartPositions)
        {
            var startPiece = board.GetPiece(possibleStartPosition);
            var relativeStartMove = new RelativeMove(possibleStartPosition, endPosition);

            if (!startPiece!.IsCorrectMovementPattern(relativeStartMove))
            {
                continue;
            }

            if (startPiece is IBlockablePiece blockablePiece && blockablePiece.CheckIfPathBlocked(
                    possibleStartPosition,
                    relativeStartMove,
                    board))
            {
                continue;
            }

            SpecialPlyAction? possibleSpecialAction = null;
            if (startPiece is ISpecialRulesPiece specialRulesPiece)
            {
                var goodSpecialMove = specialRulesPiece.SpecialRule(
                    possibleStartPosition,
                    endPosition,
                    board,
                    out possibleSpecialAction);
                if (!goodSpecialMove)
                {
                    continue;
                }
            }

            startPositionsPairs.Add(possibleStartPosition, possibleSpecialAction);
        }

        return startPositionsPairs;
    }

    private static HashSet<Position> InferPositions(UnvalidatedMove move, Board board, Player? turn)
    {
        var positions = board.GetPiecesByName(move.PieceName, turn);

        // startRow was given
        if (move.StartRow >= 0)
        {
            positions.RemoveWhere(x => x.Row != move.StartRow);
        }

        // startColumn was given
        if (move.StartColumn >= 0)
        {
            positions.RemoveWhere(x => x.Column != move.StartColumn);
        }

        return positions;
    }

    private static bool CanEscapeCheck(Board board, Player turn)
    {
        var mover = new BasicMover(board);
        var playerPositions = board.GetPiecesByPlayer(turn);
        foreach (var startPosition in playerPositions)
        {
            var possibleMoves = mover.GetPossibleMoves(startPosition, turn);
            foreach (var possibleMove in possibleMoves)
            {
                mover.Move(possibleMove);
                var escapedCheck = !CheckKingInCheck(board, turn);
                mover.Undo();

                if (escapedCheck)
                {
                    return true;
                }
            }
        }

        return false;
    }

    [GeneratedRegex(@"([0O]-[0O]-[0O])", RegexOptions.IgnoreCase)]
    private static partial Regex QueenSideCastle();
}
