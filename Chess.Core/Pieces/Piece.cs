using System.Text.Json.Serialization;

namespace Chess.Core.Pieces;

/// <summary>
/// Represents a Chess Piece.
/// </summary>
/// <param name="player">Color/ Player of this Piece.</param>
/// <param name="timesMoved">How many times this Piece has moved.</param>
[JsonDerivedType(typeof(King), "King")]
[JsonDerivedType(typeof(Queen), "Queen")]
[JsonDerivedType(typeof(Bishop), "Bishop")]
[JsonDerivedType(typeof(Knight), "Knight")]
[JsonDerivedType(typeof(Rook), "Rook")]
[JsonDerivedType(typeof(Pawn), "Pawn")]
public abstract class Piece(Player player, int timesMoved = 0)
{
    /// <summary>
    /// Display Name of this Piece.
    /// </summary>
    [JsonIgnore]
    public abstract string Name { get; }

    /// <summary>
    /// Unicode Char that represents this Piece.
    /// </summary>
    [JsonIgnore]
    public abstract char GetSymbol { get; }

    /// <summary>
    /// Character that represents this Piece according to the FEN Notation.
    /// </summary>
    [JsonIgnore]
    public abstract char GetChar { get; }

    /// <summary>
    /// Color/ Player of this Piece.
    /// </summary>
    public Player Player { get; } = player;

    /// <summary>
    /// How many times this Piece has moved.
    /// </summary>
    public int TimesMoved { get; internal set; } = timesMoved;

    /// <summary>
    /// Called to check if the current Piece is allowed to move in a way described by <paramref name="relativeMove"/>.
    /// </summary>
    /// <param name="relativeMove">Describes the way in which the Piece might move.</param>
    /// <returns>True if the described pattern matches the pattern of the Piece.</returns>
    public abstract bool IsCorrectMovementPattern(RelativeMove relativeMove);
}
