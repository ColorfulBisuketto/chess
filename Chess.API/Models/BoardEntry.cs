using Chess.Core;

namespace Chess.API.Models;

/// <summary>
/// A single Board as it is stored in the DB.
/// </summary>
public class BoardEntry
{
    /// <summary>
    /// Unique ID used as PK.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Number of Rows on the Board.
    /// </summary>
    public int Rows { get; set; }

    /// <summary>
    /// Number of Columns on the Board.
    /// </summary>
    public int Columns { get; set; }

    /// <summary>
    /// Current turn.
    /// </summary>
    public Player? Turn { get; set; }

    /// <summary>
    /// Number of moves since the last irreversible Move.
    /// </summary>
    public int ReversibleMoveNumber { get; set; }

    /// <summary>
    /// All the Squares on the board as they are stored in the DB.
    /// </summary>
    public ICollection<SquareEntry> Squares { get; set; } = new List<SquareEntry>();

    /// <summary>
    /// All Past moves as they are stored in the DB.
    /// </summary>
    public ICollection<HistoryEntry> History { get; set; } = new List<HistoryEntry>();
}
