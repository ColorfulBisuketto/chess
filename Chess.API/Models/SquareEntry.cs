namespace Chess.API.Models;

/// <summary>
/// A single Square as it is stored in the DB
/// </summary>
public class SquareEntry
{
    /// <summary>
    /// Unique ID used as PK.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// FK pointing to the BoardEntry.
    /// </summary>
    public int BoardEntryId { get; set; }

    /// <summary>
    /// BoardEntry reference.
    /// </summary>
    public BoardEntry BoardEntry { get; set; } = null!;

    /// <summary>
    /// The position of the square on the board in chess notation
    /// <example><c>"e5"</c></example>
    /// </summary>
    public string SquarePosition { get; set; }

    /// <summary>
    /// The Piece at the current position. Null if empty.
    /// <example><c>"e5"</c></example>
    /// </summary>
    public string? SquareContent { get; set; }
}
