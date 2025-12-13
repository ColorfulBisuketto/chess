using Chess.Core.Pieces;

namespace Chess.Core;

/// <summary>
/// Extension Methods for Board.
/// </summary>
public static class BoardExtensions
{
    /// <summary>
    /// Extension method to add Pieces to a board without the builder.
    /// </summary>
    /// <param name="board">This board.</param>
    /// <param name="piece">The piece to add.</param>
    /// <param name="position">The position at which to add the piece.</param>
    /// <returns>The modified board.</returns>
    public static Board AddPiece(this Board board, Piece piece, Position position)
    {
        if (board.IsValidPosition(position))
        {
            board.SetPiece(piece, position);
        }

        return board;
    }

    /// <summary>
    /// Extension method to remove Pieces from a board without the builder.
    /// </summary>
    /// <param name="board">This board.</param>
    /// <param name="position">The position at which to remove the piece from.</param>
    /// <returns>The modified board.</returns>
    public static Board RemovePiece(this Board board, Position position)
    {
        if (board.IsValidPosition(position))
        {
            board.SetPiece(null, position);
        }

        return board;
    }
}

/// <summary>
/// Builder Class used to returns instances of <c>Board</c>.
/// </summary>
/// <param name="rows">Define the number of rows.</param>
/// <param name="columns">Define the number of columns.</param>
public class BoardBuilder(int rows, int columns)
{
    private readonly Board _board = new (rows, columns);

    /// <summary>
    /// Returns a 8x8 board with all the normal Pieces on it.
    /// </summary>
    /// <returns>An instance of board.</returns>
    public static Board StandardGame()
    {
        var board = new Board(8, 8);
        board
            .AddPiece(new Rook(Player.White), new Position(0, 0))
            .AddPiece(new Knight(Player.White), new Position(0, 1))
            .AddPiece(new Bishop(Player.White), new Position(0, 2))
            .AddPiece(new Queen(Player.White), new Position(0, 3))
            .AddPiece(new King(Player.White), new Position(0, 4))
            .AddPiece(new Bishop(Player.White), new Position(0, 5))
            .AddPiece(new Knight(Player.White), new Position(0, 6))
            .AddPiece(new Rook(Player.White), new Position(0, 7));
        board
            .AddPiece(new Rook(Player.Black), new Position(7, 0))
            .AddPiece(new Knight(Player.Black), new Position(7, 1))
            .AddPiece(new Bishop(Player.Black), new Position(7, 2))
            .AddPiece(new Queen(Player.Black), new Position(7, 3))
            .AddPiece(new King(Player.Black), new Position(7, 4))
            .AddPiece(new Bishop(Player.Black), new Position(7, 5))
            .AddPiece(new Knight(Player.Black), new Position(7, 6))
            .AddPiece(new Rook(Player.Black), new Position(7, 7));
        for (var row = 0; row < 8; row++)
        {
            board.AddPiece(new Pawn(Player.White), new Position(1, row));
            board.AddPiece(new Pawn(Player.Black), new Position(6, row));
        }

        return board;
    }

    /// <summary>
    /// Returns a 8x8 board with a single Black Knight at (0, 0).
    /// </summary>
    /// <returns>An instance of board.</returns>
    public static Board KnightsTour()
    {
        var board = new Board(8, 8)
            .AddPiece(new Knight(Player.Black), new Position(0, 0));
        return board;
    }

    /// <summary>
    /// Returns a 8x8 board without any pieces on it.
    /// </summary>
    /// <returns>An instance of board.</returns>
    public static Board EmptyStandardGame()
    {
        var board = new Board(8, 8);
        return board;
    }

    /// <summary>
    /// Adds a given Piece at the specified position.
    /// </summary>
    /// <param name="piece">The piece to place.</param>
    /// <param name="position">The position to place the piece at.</param>
    /// <returns>This instance of the Builder.</returns>
    public BoardBuilder AddPiece(Piece piece, Position position)
    {
        if (_board.IsValidPosition(position))
        {
            _board.SetPiece(piece, position);
        }

        return this;
    }

    /// <summary>
    /// Removes a Piece at the specified position.
    /// </summary>
    /// <param name="position">The position to remove the piece from.</param>
    /// <returns>This instance of the Builder.</returns>
    public BoardBuilder RemovePiece(Position position)
    {
        if (_board.IsValidPosition(position))
        {
            _board.SetPiece(null, position);
        }

        return this;
    }

    /// <summary>
    /// Convert from a Builder instance to a Board instance.
    /// </summary>
    /// <returns>The current board.</returns>
    public Board Build()
    {
        return _board;
    }
}
