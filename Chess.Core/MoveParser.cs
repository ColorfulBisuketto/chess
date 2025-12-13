using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Chess.Core;

/// <summary>
/// Parses Moves represented in an algebraic chess notation to unvalidated Moves.
/// </summary>
public static partial class MoveParser
{
    /// <summary>
    /// Try to parse a movement string.
    /// return true if parseable and return the result.
    /// </summary>
    /// <param name="moveString">Move represented in algebraic chess notation.</param>
    /// <param name="move">Unvalidated move. May be Null.</param>
    /// <returns>True if the move was successfully parsed.</returns>
    public static bool TryParseMove(string moveString, [NotNullWhen(true)] out UnvalidatedMove? move)
    {
        move = null;
        if (string.IsNullOrEmpty(moveString))
        {
            return false;
        }

        // Detect castling notation
        var castle = Castle();
        var isCastle = castle.IsMatch(moveString);

        if (isCastle)
        {
            move = new UnvalidatedMove("King", -1, -1, -1, -1, string.Empty, moveString);
            return true;
        }

        // Match input using regex
        var moveRegex = MoveRegex();
        var match = moveRegex.Match(moveString);
        if (!match.Success)
        {
            return false;
        }

        var matchedPieceCode = match.Groups[1].Value;
        var matchedStartColumn = match.Groups[2].Value;
        var matchedStartRow = match.Groups[3].Value;
        var matchedEndColumn = match.Groups[4].Value;
        var matchedEndRow = match.Groups[5].Value;
        var matchedPromotionCode = match.Groups[6].Value;

        // Parse required data
        if (string.IsNullOrEmpty(matchedEndRow) || string.IsNullOrEmpty(matchedEndColumn))
        {
            return false;
        }

        var endColumn = ParseColumnLetter(matchedEndColumn);
        var endRow = ParseRowString(matchedEndRow);

        // Parse optional data
        var pieceName = string.IsNullOrEmpty(matchedPieceCode) ? "Pawn" : ParsePieceCode(matchedPieceCode);
        var startColumn = string.IsNullOrEmpty(matchedStartColumn) ? -1 : ParseColumnLetter(matchedStartColumn);
        var startRow = string.IsNullOrEmpty(matchedStartRow) ? -1 : ParseRowString(matchedStartRow);
        var promotionName = string.IsNullOrEmpty(matchedPromotionCode)
            ? string.Empty
            : ParsePieceCode(matchedPromotionCode);

        // Create return instance
        move = new UnvalidatedMove(pieceName, startRow, startColumn, endRow, endColumn, promotionName);
        return true;
    }

    private static int ParseRowString(string rowString)
    {
        return int.Parse(rowString) - 1;
    }

    private static int ParseColumnLetter(string columnString)
    {
        return (columnString[0] - 'a') % 26;
    }

    private static string ParsePieceCode(string pieceCode)
    {
        var pieceName = pieceCode switch
        {
            "K" => "King",
            "Q" => "Queen",
            "B" => "Bishop",
            "N" => "Knight",
            "R" => "Rook",
            "P" => "Pawn",
            _ => throw new ArgumentOutOfRangeException(nameof(pieceCode)),
        };
        return pieceName;
    }

    [GeneratedRegex(@"^([KQBNRP])?([a-z])?(\d+)?([a-z])(\d+)(?:=?([QBNRqbnr]))?$", RegexOptions.None, "en-DE")]
    private static partial Regex MoveRegex();

    [GeneratedRegex(@"^([0O]-[0O](?:-[0O])?)$", RegexOptions.IgnoreCase)]
    private static partial Regex Castle();
}
