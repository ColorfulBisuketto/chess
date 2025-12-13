# Basic Chess Engine

A basic chess engine written in order to learn C# and some .NET / EF-Core principles.

This repo contains:

- A class library for most of the core functionality in `./Chess.Core`.
- A cli interface to play a game of chess by yourself in `./Chess.Cli`.
  - Note that making moves is done by entering moves in the standard algebraic chess notation (e.g.: "Qe7", "e3", "Pa8Q", "Rab3").
- There is also a small class library that implements both a brute force,
  as well, as a heuristic approach at finding a "[knights tour](https://en.wikipedia.org/wiki/Knight's_tour)" for a given board in `./Chess.KnightTour`.
- The chess engine can also be run as a SignalR server in `./Chess.API`.
  - Note that the SignalR server stores currently "running" games in a sqLite file stored at `$(appDataFolder)/Chess/Chess.Board.db`
- The accompaning Angular frontend can be found in `Chess.Frontend`.
- The matching tests can be found in the `*Unit*` and `*Integration*` directories.

> [!Note]
> This was primarily written to increase my C# experience.

## Table of Contents

- [Usage](#usage)
- [License](#license)

## Usage

First ensure that you have dotnet-9.0 runtime and sdk as well as the asp-9.0 runtime installed to build or run the engine.
If you also want to use the web frontend also ensure npm and angular is installed.

Build all of the backend projects with:

```bash
dotnet build
```

---

To play a game in the cli run:

```bash
dotnet run --project Chess.Cli
```

---

To run the Web frontend first start the SignalR backend:

```bash
dotnet run --project Chess.API
```

Then to start the frontend first install the needed npm packages with:

```bash
cd Chess.Frontend
npm install
```

Before starting the Angular frontend hosted at `http://localhost:4200/`

```bash
cd Chess.Frontend
npm run start
```

---

If for some reason you want to run the tests simply run:

```bash
dotnet test
```

## License

[MIT](LICENSE) © ColorfulBisuketto
