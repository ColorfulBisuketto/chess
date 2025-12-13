using Chess.Core;

namespace Chess.API.Models;

/// <summary>
/// A single move as it is stored in the DB
/// </summary>
public class HistoryEntry
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
    /// Integer used to order History Entries.
    /// </summary>
    public int HistoryEntryNumber { get; set; }

    /// <summary>
    /// The Piece that has moved as a String.
    /// </summary>
    public string Piece { get; set; }

    /// <summary>
    /// The Pieces starting position in chess notation.
    /// <example><c>"e5"</c></example>
    /// </summary>
    public string StartPosition { get; set; }

    /// <summary>
    /// The Pieces ending position in chess notation.
    /// <example><c>"e5"</c></example>
    /// </summary>
    public string EndPosition { get; set; }

    /// <summary>
    /// The Piece that was captured in the move as a String. Null if no piece was captured.
    /// </summary>
    public string? CapturedPiece { get; set; }

    /// <summary>
    /// The special Action taken on the move. Null if not special.
    /// </summary>
    public SpecialPlyAction? SpecialPlyAction { get; set; }

    /// <summary>
    /// The Piece that the current piece was promoted to as a String. Null if the piece wasn't promoted.
    /// </summary>
    public string? PromoteToPiece { get; set; }
}
