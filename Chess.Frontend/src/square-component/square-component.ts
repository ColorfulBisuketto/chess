import {Component, computed, input, ViewChild} from '@angular/core';
import {PieceComponent} from '../piece-component/piece-component';
import {Board, FenNotationPieces} from '../board';
import {Position} from '../position';
import {GameState, StateService} from '../state-service';
import {MatMenu, MatMenuTrigger} from '@angular/material/menu';
import {CdkDrag, CdkDragDrop, CdkDropList} from '@angular/cdk/drag-drop';
import {Player} from '../player';

@Component({
  selector: 'app-square-component',
  imports: [
    PieceComponent,
    MatMenuTrigger,
    CdkDropList,
    CdkDrag
  ],
  templateUrl: './square-component.html',
  styleUrl: './square-component.css'
})
export class SquareComponent {
  state = input.required<StateService>();
  board = input.required<Board>();
  pieceCode = input.required<FenNotationPieces>();
  position = input.required<Position>();

  even = computed(()=> (this.position().row + this.position().column) % 2 == 0);
  menu = input.required<MatMenu>();

  select() {
    const board = this.board();
    const selected = board.selected();
    const position = this.position();

    if (!board.marked.has(position)) {
      if (selected.equals(position)
        || board.squares()[board.height() - position.row -1][position.column] == ' ') {
        this.clearSelected();
      } else {
        this.updateSelected();
      }
    } else {
      this.promoteOrMove(selected, position);
    }
  }

  updateSelected() {
    this.board().selected.set(this.position());
    this.state().getPossiblePositions(this.position().row, this.position().column).then();
  }
  clearSelected() {
    this.board().marked.clear();
    this.board().selected.set(new Position(-1, -1));
  }
  private promoteOrMove(startPosition: Position, endPosition: Position) {
    const board = this.board();

    const piece = board.squares()[board.height() - startPosition.row -1][startPosition.column];

    if (piece == 'P' && endPosition.row == board.height() - 1
      || piece == 'p' && endPosition.row == 0) {
      this.openPromotionMenu();
    } else {
      this.state().sendMoveWrapper(startPosition, endPosition).then();
    }

    this.clearSelected()
  }

  @ViewChild(MatMenuTrigger) trigger: MatMenuTrigger | undefined;

  openPromotionMenu() {
    this.trigger?.openMenu();
  }

  drop(event: CdkDragDrop<string[]>) {
    const board = this.board();
    const position = this.position();
    if (event.previousContainer !== event.container
      && (board.marked.has(position) || position.equals(board.selected()))) {
      this.promoteOrMove(event.item.data, this.position());
    }
  }

  isIndexZero(index: number) {
    return index === 0;
  }

  public kingsInCheck(): FenNotationPieces[] {
    if (this.state().gameState() == GameState.Check || this.state().gameState() == GameState.Checkmate) {
      switch (this.board().turn()){
        case Player.White:
          return ['K'];
        case Player.Black:
          return ['k'];
      }
    } else if (this.state().gameState() == GameState.Draw) {
      return ['k','K'];
    } else {
      return [];
    }
  }
}
