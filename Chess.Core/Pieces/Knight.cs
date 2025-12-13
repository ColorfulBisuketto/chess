using System.Text.Json.Serialization;

namespace Chess.Core.Pieces;

/// <inheritdoc />
public class Knight(Player player, int timesMoved = 0) : Piece(player, timesMoved)
{
    /// <inheritdoc/>
    [JsonIgnore]
    public override string Name => "Knight";

    /// <inheritdoc/>
    [JsonIgnore]
    public override char GetSymbol => Player == Player.White ? '♘' : '♞';

    /// <inheritdoc/>
    [JsonIgnore]
    public override char GetChar => Player == Player.White ? 'N' : 'n';

    /// <inheritdoc/>
    public override bool IsCorrectMovementPattern(RelativeMove relativeMove)
    {
        return relativeMove.RowDistance != 0 && relativeMove.ColumnDistance != 0 &&
               relativeMove.RowDistance + relativeMove.ColumnDistance == 3;
    }
}
