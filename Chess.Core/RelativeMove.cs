namespace Chess.Core;

/// <summary>
/// Initializes a new instance of the <see cref="RelativeMove"/> class.
/// </summary>
/// <param name="RowDistance">Absolute row distance.</param>
/// <param name="RowDirection">What steps to take on the row axis to reach the end position.</param>
/// <param name="ColumnDistance">Absolute column distance.</param>
/// <param name="ColumnDirection">What steps to take on the column axis to reach the end position.</param>
/// <example>
/// <list type="table">
/// <item>start: 5, end: 7 => 2x +1 steps (5 => +1 => 6 => +1 => 7)</item>
/// <item>start: 5, end: 2 => 3x -1 steps (5 => -1 => 4 => -1 => 3 => -1 => 2)</item>
/// </list>
/// </example>
public record RelativeMove(int RowDistance, int RowDirection, int ColumnDistance, int ColumnDirection)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RelativeMove"/> class based on start and end positions.
    /// </summary>
    /// <param name="startPosition">Starting position.</param>
    /// <param name="endPosition">Ending position.</param>
    public RelativeMove(Position startPosition, Position endPosition)
        : this(
            Math.Abs(endPosition.Row - startPosition.Row),
            (endPosition.Row - startPosition.Row) switch { > 0 => 1, 0 => 0, < 0 => -1 },
            Math.Abs(endPosition.Column - startPosition.Column),
            (endPosition.Column - startPosition.Column) switch { > 0 => 1, 0 => 0, < 0 => -1 })
    {
    }
}
