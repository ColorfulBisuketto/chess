using Chess.Core;

namespace Chess.API.Hubs;

public class BoardResponse
{
    public string BoardString { get; set; }

    public SimpleMove? LastMove { get; set; }

    public GameState GameState { get; set; }
}

public record SimpleMove(string From, string To);
