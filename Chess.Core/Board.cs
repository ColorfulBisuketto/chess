using System.Text;
using Chess.Core.Pieces;

namespace Chess.Core;

/// <summary>
/// The Board stores all the Pieces and their locations. Boards are rectangular.
/// </summary>
public class Board
{
    private readonly int _columns;

    private readonly int _rows;

    private HashSet<Position>? _allSquares;

    /// <summary>
    /// Initializes a new instance of the <see cref="Board"/> class.
    /// All values will be set to their default and the board will have the dimensions <paramref name="rows"/> and <paramref name="columns"/>.
    /// </summary>
    /// <param name="rows">Set the x dimension.</param>
    /// <param name="columns">Set the y dimension.</param>
    public Board(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        Squares = new Piece?[Rows * Columns];
        History = new Stack<ValidMove>();
        Turn = Player.White;
        ReversibleMoveNumber = 0;
        PastBoardOccurrences = new Dictionary<string, int>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Board"/> class.
    /// All values of the given board will be set in the new board.
    /// </summary>
    /// <param name="board">The existing board.</param>
    internal Board(Board board)
    {
        Rows = board.Rows;
        Columns = board.Columns;
        Squares = board.Squares;
        History = board.History;
        Turn = board.Turn;
        ReversibleMoveNumber = board.ReversibleMoveNumber;
        PastBoardOccurrences = board.PastBoardOccurrences;
    }

    /// <summary>
    /// The current number of rows.
    /// </summary>
    public int Rows
    {
        get => _rows;
        private init
        {
            if (value == _rows)
            {
                return;
            }

            _rows = value;
            _allSquares = null;
        }
    }

    /// <summary>
    /// The Current number of columns.
    /// </summary>
    public int Columns
    {
        get => _columns;
        private init
        {
            if (value == _columns)
            {
                return;
            }

            _columns = value;
            _allSquares = null;
        }
    }

    /// <summary>
    /// Stores all Valid moves previously made.
    /// </summary>
    public Stack<ValidMove> History { get; set; }

    /// <summary>
    /// Enumerable to more easily loop over all squares on the board.
    /// </summary>
    public IEnumerable<Position> AllSquares
    {
        get
        {
            _allSquares ??= Enumerable.Range(0, Rows)
                .SelectMany(x =>
                    Enumerable.Range(0, Columns).Zip(Enumerable.Repeat(x, Columns), (p1, p2) => new Position(p1, p2)))
                .ToHashSet();
            return _allSquares;
        }
    }

    /// <summary>
    /// Stores which players turn it currently is.
    /// </summary>
    public Player? Turn { get; internal set; }

    /// <summary>
    /// <para>Number of moves since the last irreversible move.</para>
    /// <para>
    /// A reversible move is any move that can be undone by simply moving all the pieces back into position.
    /// Moves capture other pieces or move pawns are irreversible since pawns can ony ever move in one direction.
    /// </para>
    /// </summary>
    public int ReversibleMoveNumber { get; set; }

    /// <summary>
    ///     Stores a Shortened FEN Notation string, without turn/ reversible move numbers,
    ///     along with how many times this board has occured during a Game.
    ///     This is needed to track threefold repetition.
    /// </summary>
    public Dictionary<string, int> PastBoardOccurrences { get; set; }

    /// <summary>
    /// Internal Unique ID used as PK when storing boards the wrong way.
    /// </summary>
    internal int Id { get; init; }

    /// <summary>
    /// Internal storage of all Pieces. Conversion between 2D representation and 1D data is done using:
    /// <c>index = (currentRow * numberOfRows) + currentColumn</c>.
    /// </summary>
    internal Piece?[] Squares { get; set; }

    /// <summary>
    /// Returns a string that represents the current object in the FEN Notation.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        var output = new StringBuilder();

        output.Append(PiecePlacementString()); // Board State

        output.Append(' ');
        output.Append(
            Turn switch
            {
                Player.White => 'w',
                Player.Black => 'b',
                null => "x",
                _ => throw new ArgumentOutOfRangeException(nameof(Turn)),
            }); // Current Player

        output.Append(' ');
        output.Append(CastlingRightsString()); // Castling Rights

        output.Append(' ');
        output.Append(EnPassantTargetString()); // En Passant Targets

        output.Append(' ');
        output.Append(ReversibleMoveNumber); // number of half moves

        output.Append(' ');
        output.Append(Math.Floor((double)History.Count / 2) + 1); // Current Move Number

        return output.ToString();
    }

    /// <summary>
    /// Get the position that a pawn can be captured from using an en passant capture if possible.
    /// </summary>
    /// <returns>Null if no en passant capture is possible.</returns>
    public Position? GetPossibleEnPassantCapturePosition()
    {
        var hasHistory = History.TryPeek(out var lastMove);
        if (!hasHistory || lastMove is not { Piece: Pawn })
        {
            return null;
        }

        var relativeMove = new RelativeMove(lastMove.StartPosition, lastMove.EndPosition);
        if (relativeMove.RowDistance != 2)
        {
            return null;
        }

        return lastMove.StartPosition with { Row = lastMove.StartPosition.Row + relativeMove.RowDirection };
    }

    /// <summary>
    /// Chacks is the given position is within the bounds of the board.
    /// </summary>
    /// <param name="position">The position in question.</param>
    /// <returns>True if within the bound of the board.</returns>
    public bool IsValidPosition(Position position)
    {
        return position.Row >= 0 && position.Row < Rows && position.Column >= 0 && position.Column < Columns;
    }

    /// <summary>
    /// Gets whatever Piece is at the specified position.
    /// </summary>
    /// <param name="position">The position in question.</param>
    /// <returns>A Piece from the position. Null if no Piece was at position.</returns>
    public Piece? GetPiece(Position position)
    {
        return !IsValidPosition(position) ? null : Squares[(position.Row * Rows) + position.Column];
    }

    /// <summary>
    /// Gets whatever Piece is at the specified x, y position.
    /// </summary>
    /// <param name="row">The row in question.</param>
    /// <param name="column">The column in question.</param>
    /// <returns>A Piece from the position. Null if no Piece was at position.</returns>
    public Piece? GetPiece(int row, int column)
    {
        return !IsValidPosition(new Position(row, column)) ? null : Squares[(row * Rows) + column];
    }

    /// <summary>
    /// Place a piece at a Position.
    /// </summary>
    /// <param name="piece">The Piece to place</param>
    /// <param name="position">The position to place the piece.</param>
    public void SetPiece(Piece? piece, Position position)
    {
        if (!IsValidPosition(position))
        {
            return;
        }

        Squares[(position.Row * Rows) + position.Column] = piece;
    }

    /// <summary>
    /// Gat all positions of pieces with a specified name and optionally color.
    /// </summary>
    /// <param name="pieceName">The name to search for.</param>
    /// <param name="player">Optional color of the pieces.</param>
    /// <returns>A Set with all positions that have a piece matching the given name and player.</returns>
    public HashSet<Position> GetPiecesByName(string pieceName, Player? player = null)
    {
        var piecePositions = new HashSet<Position>();
        foreach (var position in AllSquares)
        {
            var piece = GetPiece(position);
            if (piece == null)
            {
                continue;
            }

            if ((player == null && piece.Name == pieceName)
                || (piece.Player == player && piece.Name == pieceName))
            {
                piecePositions.Add(position);
            }
        }

        return piecePositions;
    }

    /// <summary>
    /// Gat all positions of pieces with a specified color.
    /// </summary>
    /// <param name="player">Color of the pieces to search for.</param>
    /// <returns>A Set with all positions that have a piece matching the given color.</returns>
    public HashSet<Position> GetPiecesByPlayer(Player player)
    {
        var piecePositions = new HashSet<Position>();
        foreach (var position in AllSquares)
        {
            var piece = GetPiece(position);
            if (piece == null)
            {
                continue;
            }

            if (piece.Player == player)
            {
                piecePositions.Add(position);
            }
        }

        return piecePositions;
    }

    private string PiecePlacementString()
    {
        var piecesBuilder = new StringBuilder();

        for (var row = Rows - 1; row >= 0; row--)
        {
            var emptyCounter = 0;

            for (var column = 0; column < Columns; column++)
            {
                var piece = Squares[(row * Rows) + column];

                if (piece == null)
                {
                    emptyCounter++;
                }
                else
                {
                    if (emptyCounter > 0)
                    {
                        piecesBuilder.Append(emptyCounter);
                        emptyCounter = 0;
                    }

                    piecesBuilder.Append(piece.GetChar);
                }
            }

            if (emptyCounter > 0)
            {
                piecesBuilder.Append(emptyCounter);
            }

            if (row > 0)
            {
                piecesBuilder.Append('/');
            }
        }

        return piecesBuilder.ToString();
    }

    private string CastlingRightsString()
    {
        var castlingRightsBuilder = new StringBuilder();

        var kingPositions = GetPiecesByName("King");

        foreach (var kingPosition in kingPositions)
        {
            var king = GetPiece(kingPosition);
            if (king is not { TimesMoved: 0 })
            {
                continue;
            }

            var rookPositions = GetPiecesByName("Rook", king.Player);
            foreach (var rookPosition in rookPositions)
            {
                var rookPiece = GetPiece(rookPosition);
                if (rookPiece is not { TimesMoved: 0 })
                {
                    continue;
                }

                var relativeRook = new RelativeMove(kingPosition, rookPosition);
                castlingRightsBuilder.Append(
                    relativeRook.ColumnDirection switch
                    {
                        -1 => new King(king.Player).GetChar,
                        1 => new Queen(king.Player).GetChar,
                        _ => string.Empty,
                    });
            }
        }

        if (castlingRightsBuilder.Length == 0)
        {
            castlingRightsBuilder.Append('-');
        }

        return castlingRightsBuilder.ToString();
    }

    private string EnPassantTargetString()
    {
        var enPassantTarget = GetPossibleEnPassantCapturePosition();
        return enPassantTarget is null ? "-" : ((Position)enPassantTarget).ToString();
    }
}
