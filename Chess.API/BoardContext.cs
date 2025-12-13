using System.Text.Json;
using Chess.API.Models;
using Chess.Core;
using Chess.Core.Pieces;
using Microsoft.EntityFrameworkCore;

namespace Chess.API;

/// <inheritdoc />
public class BoardContext : DbContext
{
    /// <inheritdoc />
    public BoardContext()
    {
        const Environment.SpecialFolder appDataFolder = Environment.SpecialFolder.LocalApplicationData;
        var appDataPath = Environment.GetFolderPath(appDataFolder);
        var chessFolder = Path.Combine(appDataPath, "Chess");
        Directory.CreateDirectory(chessFolder);

        DbPath = Path.Join(chessFolder, "Chess.Boards.db");
    }

    /// <summary>
    /// Table to hold Board Objects directly.
    /// </summary>
    public DbSet<Board> Boards { get; init; }

    /// <summary>
    /// Defines a table that holds BoardEntries.
    /// </summary>
    public DbSet<BoardEntry> BoardEntries { get; init; }

    private string DbPath { get; }

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DbPath}");
    }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // TODO: Prefer using BoardEntity (Split Entity and Logic)
        modelBuilder.Entity<Board>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Squares)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<Piece?[]>(v, JsonSerializerOptions.Default) ??
                         Array.Empty<Piece?>());
            e.Property(x => x.History)
                .HasConversion(
                    v => JsonSerializer.Serialize(v.Reverse(), JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<Stack<ValidMove>>(v, JsonSerializerOptions.Default) ??
                         new Stack<ValidMove>());
            e.Property(x => x.PastBoardOccurrences)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<Dictionary<string, int>>(v, JsonSerializerOptions.Default) ??
                         new Dictionary<string, int>());
            e.Ignore(x => x.AllSquares);
        });
    }
}
