import {Component, input} from '@angular/core';
import {FenNotationPieces} from '../board';
import {CdkDragPlaceholder} from '@angular/cdk/drag-drop';

const PieceSymbols: Record<FenNotationPieces, string> = {
  'k': 'kd',
  'q': 'qd',
  'b': 'bd',
  'n': 'nd',
  'r': 'rd',
  'p': 'pd',
  'K': 'kl',
  'Q': 'ql',
  'B': 'bl',
  'N': 'nl',
  'R': 'rl',
  'P': 'pl',
  ' ': ' '
} as const;

@Component({
  selector: 'app-piece-component',
  imports: [
    CdkDragPlaceholder
  ],
  templateUrl: './piece-component.html',
  styleUrl: './piece-component.css'
})
export class PieceComponent {
  protected readonly FenNotationDict = PieceSymbols;

  pieceCode = input.required<FenNotationPieces>();
}
