using System.Text.RegularExpressions;

namespace Chess.Core;

/// <summary>
/// <para>Structure to define a position.</para>
/// <para>Negative parameters represent unknowns or non-existing positions.</para>
/// </summary>
/// <param name="Row">Integer to represent the y-axis.</param>
/// <param name="Column">Integer to represent the x-axis.</param>
public readonly partial record struct Position(int Row, int Column)
{
    /// <summary>
    /// Turn positions into their chess notation equivalent.
    /// <list type="bullet">
    /// <item>
    /// In chess column/file is written before row/rank.
    /// </item>
    /// <item>
    /// rows start at 1 not 0.
    /// </item>
    /// <item>
    /// columns are written as chars a-z with a being the lowest (97 is the ascii offset for 'a' with there being 26 letters in total).
    /// </item>
    /// </list>
    /// </summary>
    /// <example>row: 0, column: 3 => "c1".</example>
    /// <returns>String representation of the current object.</returns>
    public override string ToString()
    {
        return $"{(char)((Column % 26) + 97)}{Row + 1}";
    }

    /// <summary>
    /// Get an instance of position from a position written in chess notation.
    /// </summary>
    /// <param name="dirtyPositionString">String in chess notation.</param>
    /// <returns>A position parsed from the string. Returns <c>new Position(-1, -1)</c> (or <c>"`0"</c> as string) if not parseable.</returns>
    public static Position FromString(string dirtyPositionString)
    {
        var cleanPositionString = dirtyPositionString.Trim().ToLower();
        var reg = PositionRegex();
        var isMatch = reg.IsMatch(cleanPositionString);

        return isMatch
            ? new Position(int.Parse(cleanPositionString[1..]) - 1, cleanPositionString[0] - 97)
            : new Position(-1, -1);
    }

    [GeneratedRegex(@"^[A-Z]\d+$", RegexOptions.IgnoreCase, "en-DE")]
    private static partial Regex PositionRegex();
}
