namespace Chess.Core.Pieces;

/// <summary>
/// Used to Define if a Piece has any special rules for its movement that cannot be described by <c>IsCorrectMovementPattern</c>.
/// Outputs a specialPlyAction for a move if a special action has to be taken or remembered for later.
/// </summary>
public interface ISpecialRulesPiece
{
    /// <summary>
    /// Used to check any special Piece Rules and return a specialPlyAction if needed.
    /// </summary>
    /// <param name="startPosition">Starting Position of the movement.</param>
    /// <param name="endPosition">Ending Position of the movement.</param>
    /// <param name="board">The board on which to move</param>
    /// <param name="specialAction">The specialPlyAction that has to be taken. Null if not special.</param>
    /// <returns>True if the special rules are held.</returns>
    bool SpecialRule(Position startPosition, Position endPosition, Board board, out SpecialPlyAction? specialAction);

    /// <summary>
    /// Used if special Actions have to be taken when special moves are made.
    /// </summary>
    /// <param name="move">The move that is being made.</param>
    /// <param name="board">The board on which to move.</param>
    void SpecialMove(ValidMove move, Board board);

    /// <summary>
    /// Used if special Actions have to be taken when undoing special moves.
    /// </summary>
    /// <param name="lastMove">The last move that was is made.</param>
    /// <param name="board">The board on which to undo.</param>
    void SpecialUndo(ValidMove lastMove, Board board);
}
