using System.Text.Json.Serialization;

namespace Chess.Core.Pieces;

/// <inheritdoc cref="Piece" />
public class Queen(Player player, int timesMoved = 0) : Piece(player, timesMoved), IBlockablePiece
{
    /// <inheritdoc/>
    [JsonIgnore]
    public override string Name => "Queen";

    /// <inheritdoc/>
    [JsonIgnore]
    public override char GetSymbol => Player == Player.White ? '♕' : '♛';

    /// <inheritdoc/>
    [JsonIgnore]
    public override char GetChar => Player == Player.White ? 'Q' : 'q';

    /// <inheritdoc/>
    public override bool IsCorrectMovementPattern(RelativeMove relativeMove)
    {
        return relativeMove.RowDistance == 0 || relativeMove.ColumnDistance == 0 ||
               relativeMove.RowDistance == relativeMove.ColumnDistance;
    }
}
