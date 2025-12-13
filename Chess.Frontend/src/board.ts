import {Player} from './player';
import {CastlingRights} from './castling-rights';
import {Position} from './position';
import {PositionSet} from './positionSet';
import {computed, signal} from '@angular/core';

export type FenNotationPieces = 'k' | 'q' | 'b' | 'n' | 'r' | 'p' | 'K' | 'Q' | 'B' | 'N' | 'R' | 'P' | ' ' ;

export class Board{
  squares = signal<FenNotationPieces[][]>([[]]);
  turn = signal(Player.White);
  halfMoveNumber = signal(0);
  fullMoveNumber = signal(0);
  selected = signal(new Position(-1,-1));

  height = computed(()=>this.squares().length);
  width =  computed(()=>this.squares()[0].length);

  castling = new CastlingRights();
  enPassants = new PositionSet();
  marked = new PositionSet();
  lastMove = new PositionSet();

  // See https://de.wikipedia.org/wiki/Forsyth-Edwards-Notation for more info on the Notation.
  setBoard(fenNotation: string) {
    const fenNotationArray = fenNotation.split(' ');

    this.setPieces(fenNotationArray[0]);
    this.setTurn(fenNotationArray[1]);
    this.setCastlingRights(fenNotationArray[2]);
    this.setEnPassant(fenNotationArray[3]);
    this.setHalfMoveNumber(fenNotationArray[4]);
    this.setFullMoveNumber(fenNotationArray[5]);
  }

  private setPieces(input: string) {
    const empty = ' ';
    const matches = input.matchAll(/\d+/g);
    for(const match of matches) {
      input = input
        .replace(match[0], empty.repeat(parseInt(match[0])));
    }

    this.squares.set(input
      .split('/')
      .map(row => row // Turn full strings into an array of 'chars'
        .split('')) as FenNotationPieces[][]);
  }
  private setTurn(input: string) {
    this.turn.set((input == "w") ? Player.White : Player.Black);
  }
  private setCastlingRights(input: string) {
    this.castling.whiteKingSide.set(input.includes("K"));
    this.castling.whiteQueenSide.set(input.includes("Q"));
    this.castling.blackKingSide.set(input.includes("k"));
    this.castling.blackQueenSide.set(input.includes("q"));
  }
  private setEnPassant(input: string) {
    this.enPassants.clear();
    if (input == "-") return;
    this.enPassants.add(Position.fromString(input));
  }
  private setHalfMoveNumber(input: string) {
    this.halfMoveNumber.set(parseInt(input));
  }
  private setFullMoveNumber(input: string) {
    this.fullMoveNumber.set(parseInt(input));
  }

  static fromString(fenNotation: string): Board {
    const board = new Board();
    board.setBoard(fenNotation);
    return board;
  }
}
