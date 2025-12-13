import {signal} from '@angular/core';

export class CastlingRights{
  whiteKingSide  = signal(false);
  whiteQueenSide = signal(false);
  blackKingSide  = signal(false);
  blackQueenSide = signal(false);
}
