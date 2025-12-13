namespace Chess.Core;

/// <summary>
/// A move that has not yet been validated.
/// </summary>
/// <param name="PieceName">The name of the piece to move.</param>
/// <param name="StartRow">The starting row. <c>-1</c> if unknown.</param>
/// <param name="StartColumn">The starting column. <c>-1</c> if unknown.</param>
/// <param name="EndRow">The ending column. <c>-1</c> if unknown.</param>
/// <param name="EndColumn">The ending column. <c>-1</c> if unknown.</param>
/// <param name="PromotionName">The name of the piece that the current piece should promote into.</param>
/// <param name="CastleString">The string used to castle. Optional.</param>
public record UnvalidatedMove(
    string PieceName,
    int StartRow,
    int StartColumn,
    int EndRow,
    int EndColumn,
    string PromotionName,
    string CastleString = "")
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnvalidatedMove"/> class based on positions.
    /// </summary>
    /// <param name="PieceName">The name of the piece to move.</param>
    /// <param name="StartPosition">The starting position. values of <c>-1</c> if value is unknown.</param>
    /// <param name="EndPosition">The ending position. values of <c>-1</c> if value is unknown.</param>
    /// <param name="PromotionName">The name of the piece that the current piece should promote into.</param>
    /// <param name="CastleString">The string used to castle. Optional.</param>
    public UnvalidatedMove(string PieceName, Position StartPosition, Position EndPosition, string PromotionName, string CastleString = "")
        : this(PieceName, StartPosition.Row, StartPosition.Column, EndPosition.Row, EndPosition.Column, PromotionName, CastleString)
    {
    }
}
