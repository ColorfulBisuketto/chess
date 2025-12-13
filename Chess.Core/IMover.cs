namespace Chess.Core;

/// <summary>
/// Defines methods manipulate a given board.
/// </summary>
public interface IMover
{
    /// <summary>
    /// The board that should be bound to the mover.
    /// </summary>
    public Board Board { get; }

    /// <summary>
    /// Return an Array of all position reachable by the piece currently at the starting position.
    /// </summary>
    /// <param name="startPosition">The starting position.</param>
    /// <returns>An Array of possible ending position. Empty if the starting square is empty or out of the bounds of the board.</returns>
    public Position[] GetPossiblePositions(Position startPosition);

    /// <summary>
    /// Move all the relevant pieces on the board in order to execute a valid move.
    /// </summary>
    /// <param name="validMove">A move that has already been validated.</param>
    /// <returns>True if modification of the board was a success.</returns>
    public bool Move(ValidMove validMove);

    /// <summary>
    /// Move all the relevant pieces on the board in order to undo the last move.
    /// </summary>
    public void Undo();
}
