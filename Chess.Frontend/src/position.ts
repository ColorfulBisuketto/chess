export class Position{
  public row: number;
  public column: number;

  constructor(row: number, column: number) {
    this.row = row;
    this.column = column;
  }

  public equals(position: Position | null | undefined): boolean {
    if (position === null || position === undefined) return false;
    return (this.column === position.column && this.row === position.row);
  }

  public toString(): string {
    // Turn positions into their algebraic chess notation equivalent
    // Note: In chess column/file is written before row/rank.
    //       rows start at 1 not 0.
    //       columns are written as chars a-z with a being the lowest (97 is the ascii offset for 'a' with there being 26 letters in total)
    // E.g.: row: 0, column: 3 => "c1"
    return `${String.fromCharCode(this.column + 97)}${this.row + 1}`;
  }

  static fromString(input: string): Position{
    const match = input.match(/[a-z]\d+/);
    if (match != null && match.length >= 1) {
      const column = match[0].toUpperCase().charCodeAt(0) - 65;   // turn letters into numbers (A - Z => 0 - 26)
      const row = parseInt(match[0].slice(1)) - 1;                // turn one indexed number int zero indexed number (0 - 7 => 1 - 8)

      return new Position(row, column);
    }
    throw new Error("Unknown position");
  }
}
