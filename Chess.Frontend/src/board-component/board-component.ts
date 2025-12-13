import {Component, computed, input} from '@angular/core';
import {SquareComponent} from '../square-component/square-component';
import {Board, FenNotationPieces} from '../board';
import {Player} from '../player';
import {Position} from '../position';
import {StateService} from '../state-service';
import {MatMenu} from '@angular/material/menu';
import {CdkDropListGroup} from '@angular/cdk/drag-drop';

@Component({
  selector: 'app-board-component',
  imports: [
    SquareComponent,
    CdkDropListGroup,
  ],
  templateUrl: './board-component.html',
  styleUrl: './board-component.css'
})
export class BoardComponent {
  protected readonly String = String; // Needed to convert from number to ascii char in template.

  state = input.required<StateService>();
  board = input.required<Board>();
  currentPlayer = input<Player>(Player.White);
  menu = input.required<MatMenu>();

  displayedBoardArray = computed((): FenNotationPieces[][] => {
    return (this.currentPlayer() === Player.White)
      ? this.board().squares().slice()
      : this.board().squares().slice().reverse().map(value => value.slice().reverse());
  });

  public translatePosition(row: number, column: number):Position {
    return (this.currentPlayer() === Player.White)
      ? new Position(this.board().height() - row -1, column)
      : new Position(row, this.board().width() - column -1);
  }
}
