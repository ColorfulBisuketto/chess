using System.Text;
using Chess.Cli;
using Chess.Core;

Console.OutputEncoding = Encoding.UTF8;

var board = BoardBuilder.StandardGame();

var game = new GameMover(board);
var display = new BoardRenderer(game.Board);

while (true)
{
    Console.Clear();
    display.Render();
    Console.Write($"Turn for {(game.Board.Turn is null ? "All" : game.Board.Turn)} (q to quit): ");
    while (true)
    {
        var input = Console.ReadLine();
        if (input is "q" or "Q")
        {
            return;
        }

        if (input != null && game.Move(input))
        {
            break;
        }

        Console.Write($"Invalid move {input}. Please try again : ");
    }
}
