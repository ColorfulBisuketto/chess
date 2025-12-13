using System.Text.Json;
using Chess.API;
using Chess.API.Hubs;
using Chess.Core;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAngularLocalHost",
        policyBuilder =>
            policyBuilder.WithOrigins("http://localhost:4200")
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod());
});
builder.Services.AddDbContext<BoardContext>();

var app = builder.Build();

// Ensure DB exists
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BoardContext>();

    // If you're not using migrations, use EnsureCreated():
    // db.Database.EnsureCreated();

    // If you add migrations later, switch to:
    db.Database.Migrate();
}

app.UseCors("AllowAngularLocalHost");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

// Map Endpoints
app.MapGet(
        "/games",
        async () =>
        {
            var db = new BoardContext();
            var boards = await db.Boards.Select(board => board.Id).ToArrayAsync();
            await db.DisposeAsync();
            return JsonSerializer.Serialize(boards);
        })
    .WithName("GetGames");

app.MapGet(
        "/newGame",
        async () =>
        {
            var board = BoardBuilder.StandardGame();
            var db = new BoardContext();
            db.Boards.Add(board);
            await db.SaveChangesAsync();
            await db.DisposeAsync();
            return JsonSerializer.Serialize(board.Id);
        })
    .WithName("CreateNewGame");

app.MapHub<ChessHub>("/chessHub/{GameId}");

app.Run();
