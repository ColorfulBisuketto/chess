using System.Text.Json.Serialization;

namespace Chess.Core.Pieces;

/// <inheritdoc cref="Piece" />
public class Rook(Player player, int timesMoved = 0) : Piece(player, timesMoved), IBlockablePiece
{
    /// <inheritdoc/>
    [JsonIgnore]
    public override string Name => "Rook";

    /// <inheritdoc/>
    [JsonIgnore]
    public override char GetSymbol => Player == Player.White ? '♖' : '♜';

    /// <inheritdoc/>
    [JsonIgnore]
    public override char GetChar => Player == Player.White ? 'R' : 'r';

    /// <inheritdoc/>
    public override bool IsCorrectMovementPattern(RelativeMove relativeMove)
    {
        return relativeMove.RowDistance == 0 || relativeMove.ColumnDistance == 0;
    }
}
