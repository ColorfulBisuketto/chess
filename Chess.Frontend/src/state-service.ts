import { computed, Inject, Injectable, signal, untracked } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Board } from './board';
import { Position } from './position';
import { IChessHubClient } from './ichess-hub-client';

export enum StatusCode {
  Empty = 0,
  Success = 200,
  BadRequest = 400,
  NotFound = 404,
  Error = 500
}
export enum GameState {
  Running = 0,
  Check = 1,
  Checkmate = 2,
  Draw = 3,
}

export type SimpleMove = { from: string, to: string };
export type undoRequest = {};
export type deleteRequest = {};
export type MoveRequest = { moveString: string };
export type possiblePositionsRequest = { row: number, column: number };
export type BoardResponse = { boardString: string, lastMove: SimpleMove, gameState: GameState };
export type PossiblePositionsResponse = { possiblePositionStrings: string[] };
export type StatusResponse = { code: StatusCode; message: string };

@Injectable({
  providedIn: 'root'
})
export class StateService implements IChessHubClient {
  board = signal<Board | null>(null);
  status = signal<StatusResponse>({ code: StatusCode.Empty, message: "Board received." });
  connectionState = signal<StatusCode>(StatusCode.Empty);
  gameState = signal<GameState>(GameState.Running);

  colorCode = computed(() => {
    const board = this.board();
    const selected = board?.selected();
    const squares = board?.squares();
    return untracked(() => {
      if (selected != undefined && squares != undefined && !selected.equals(new Position(-1, -1))) {
        return squares[selected.row][selected.column] == 'P' ? 'l' : 'd';
      }
      return 'l';
    });
  });

  private hubConnection!: signalR.HubConnection;
  private connectionStopped = false;
  private moveSound = new Audio('/move.mp3');

  gameId: number;

  constructor(
    @Inject('gameId') public gameIdString: string,
  ) {
    this.gameId = parseInt(gameIdString);
    this.moveSound.load();

    this.startConnection();
    this.addBoardListener();
    this.handleDisconnects();
  }

  // SignalR
  startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`http://localhost:5192/chessHub/${this.gameIdString}`)
      .build();
    this.hubConnection
      .start()
      .then(() => {
        console.log('Connection started');
        this.connectionState.set(StatusCode.Success);
      })
      .catch((err) => {
        console.log("Error while establishing connection: " + err);
        this.connectionState.set(StatusCode.Error);
      });
  }

  // SignalR
  stopConnection = () => {
    this.connectionStopped = true;
    this.hubConnection.stop().then(() => console.log("Connection stopped"));
  }

  // SignalR
  handleDisconnects = () => {
    this.hubConnection.onclose(() => {
      if (!this.connectionStopped) {
        console.log('Connection lost. Attempting to reconnect...');
        this.connectionState.set(StatusCode.Error);
        setTimeout(() => this.startConnection(), 3000);
      }
    });
  }

  // SignalR
  addBoardListener = () => {
    this.hubConnection.on('ReceiveBoard', (res: BoardResponse) => {
      this.updateBoard(res.boardString);
      this.board()?.lastMove.clear()
      this.board()?.lastMove.add(Position.fromString(res.lastMove.from));
      this.board()?.lastMove.add(Position.fromString(res.lastMove.to));
      this.gameState.set(res.gameState);
      this.status.set({ code: StatusCode.Success, message: "Board received" })
    });
  }

  // SignalR
  async sendMove(moveString: string) {
    const req: MoveRequest = { moveString: moveString };
    this.board()?.marked.clear();
    this.board()?.selected.set(new Position(-1, -1));
    this.moveSound.play().then();
    this.hubConnection.invoke('SendMove', req)
      .then((res: StatusResponse) => {
        this.status.set(res);
      })
      .catch(err => console.error(err));
  }

  // SignalR
  async getPossiblePositions(row: number, column: number) {
    const req: possiblePositionsRequest = { row: row, column: column };
    this.hubConnection.invoke('GetPossiblePositions', req)
      .then((res: PossiblePositionsResponse) => {
        this.board.update(value => {
          if (value == null) {
            return null;
          } else {
            const board = value;
            board.marked.clear();
            board.marked.addMany(res.possiblePositionStrings
              .map((positionString: string) => Position.fromString(positionString)));
            return board;
          }
        });
      })
      .catch(err => console.error(err));
  }

  // SignalR
  async undoMove() {
    const req: undoRequest = {};
    this.board()?.marked.clear();
    this.board()?.selected.set(new Position(-1, -1));
    this.hubConnection.invoke('UndoMove', req)
      .then((res: StatusResponse) => {
        this.status.set(res);
      })
      .catch(err => console.error(err));
  }

  // SignalR
  async deleteGame() {
    const req: deleteRequest = {};
    await this.hubConnection.invoke('DeleteGame', req)
      .then((res: StatusResponse) => {
        this.status.set(res);
        this.board.set(null);
      })
      .catch(err => console.error(err));
  }

  async sendMoveWrapper(startPosition: Position, endPosition: Position, promoteToPiece: string = "") {
    if (this.board() != null) {
      const pieceCode = this.board()!.squares()[this.board()!.height() - startPosition.row - 1][startPosition.column].toUpperCase();
      const moveString = `${pieceCode}${startPosition.toString()}${endPosition.toString()}${promoteToPiece}`;
      await this.sendMove(moveString);
    }
  }

  private updateBoard(boardString: string): void {
    this.board.update(value => {
      if (value == null) {
        return Board.fromString(boardString);
      } else {
        const selected = value.selected();
        const board = Board.fromString(boardString);
        board.selected.set(selected);
        return board;
      }
    });
  }
}
