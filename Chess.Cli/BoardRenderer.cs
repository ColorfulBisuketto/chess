using System.Text;
using Chess.Core;

namespace Chess.Cli;

/// <summary>
/// Class to render a Board on the Console.
/// </summary>
public class BoardRenderer(Board board)
{
    private Board Board { get; set; } = board;

    /// <summary>
    /// Renders a given <paramref name="board"/> to the Console.
    /// </summary>
    /// <param name="board">The board to render.</param>
    public static void Render(Board board)
    {
        var outputBuilder = new StringBuilder().AppendLine();
        var rows = board.Rows;
        var columns = board.Columns;
        var separatorCount = (columns * 6) + 3;
        const string rowFormat = "{0,2} ";
        const string squareFormat = "│  {0}  ";
        const string emptySquareFormat = "│{0,5}";
        const string whiteSquare = " ███ ";
        const string blackSquare = "     ";

        for (var row = rows - 1; row >= 0; row--)
        {
            outputBuilder.AppendFormat(rowFormat, row + 1);
            for (var col = 0; col < columns; col++)
            {
                var piece = board.GetPiece(row, col);
                if (piece == null)
                {
                    outputBuilder.AppendFormat(emptySquareFormat, (row + col) % 2 == 0 ? whiteSquare : blackSquare);
                }
                else
                {
                    outputBuilder.AppendFormat(squareFormat, piece.GetSymbol);
                }
            }

            outputBuilder.AppendLine().Append('─', separatorCount).AppendLine();
        }

        outputBuilder.Append(' ', 3);
        for (var col = 0; col < columns; col++)
        {
            outputBuilder.AppendFormat(squareFormat, (char)((col % 26) + 97));
        }

        Console.WriteLine(outputBuilder.ToString());
        Console.WriteLine();
    }

    /// <summary>
    /// Render the current board to the Console.
    /// </summary>
    public void Render()
    {
        Render(Board);
    }
}
