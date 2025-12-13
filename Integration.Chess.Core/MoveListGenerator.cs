using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Chess.Core;

namespace Integration.Chess.Core;

/// <summary>
/// Helper Class used to Play entire Games stored in simplified PGN (Portable Game Notation).
///
/// <para>Doesn't support the result at the end of the match ("1/2-1/2", "1-0", "0-1").</para>
/// </summary>
public static partial class MoveListGenerator
{
    /// <summary>
    /// Using a PGN string Play an assortment of moves and test if special actions have been made.
    /// If an error is detected return false immediately and set failedMove.
    ///
    /// <para>Doesn't support the result at the end of the match ("1/2-1/2", "1-0", "0-1").</para>
    /// </summary>
    /// <param name="gameMover">The Mover to be tested.</param>
    /// <param name="moveList">A string of Moves in PGN.</param>
    /// <param name="failedMove">If there was an error with a move return that specific move string else return <c>Null</c>.</param>
    /// <returns>return true if all moves in moveList can be played correctly, false otherwise.</returns>
    public static bool PlayMany(GameMover gameMover, string moveList, [NotNullWhen(false)] out string? failedMove)
    {
        var ignoreRegex = Ignore();
        var hintRegex = Hint();
        var castleRegex = Castle();

        var moves = ignoreRegex.Replace(moveList, string.Empty).Split([' ', '\n'], StringSplitOptions.RemoveEmptyEntries);
        foreach (var move in moves)
        {
            var isCheck = move.Contains('+');
            var isCheckMate = move.Contains('#');
            var isCapture = move.Contains('x');
            var isCastle = castleRegex.IsMatch(move);

            var cleanMove = hintRegex.Replace(move, string.Empty);

            var isValid = gameMover.Move(cleanMove);

            var lastMove = gameMover.Board.History.Peek();
            if (!isValid
                || (isCheck && gameMover.State != GameState.Check)
                || (isCheckMate && gameMover.State != GameState.CheckMate)
                || (isCapture && lastMove.CapturedPiece == null)
                || (isCastle && lastMove.SpecialPlyAction != SpecialPlyAction.Castle))
            {
                failedMove = move;
                return false;
            }
        }

        failedMove = null;
        return true;
    }

    [GeneratedRegex(@"([\+|x|#])")]
    private static partial Regex Hint();

    [GeneratedRegex(@"(\d+\.+)|({.*?})|(\[.*?\])|(;.*?\n)")]
    private static partial Regex Ignore();

    [GeneratedRegex(@"([0O]-[0O](?:-[0O])?)")]
    private static partial Regex Castle();
}
