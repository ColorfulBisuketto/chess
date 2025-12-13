import {Position} from './position';

// not a typescript set because object equality would require a custom class anyway.

export class PositionSet {
  private positions: Position[] = [];

  constructor(positions: Position[] = []) {
    this.positions = positions;
  }

  add(item: Position): void {
    if (!this.positions.some(existing => existing.equals(item))) {
      this.positions.push(item);
    }
  }

  addMany(items: Position[]): void {
    const arr = items.filter(item => !this.positions.some(existing => existing.equals(item)));
    this.positions.push(...arr);
  }

  has(item: Position): boolean {
    return (this.positions.some(existing => existing.equals(item)));
  }

  clear(): void {
    this.positions = [];
  }

  values(): Position[] {
    return [...this.positions];
  }
}
