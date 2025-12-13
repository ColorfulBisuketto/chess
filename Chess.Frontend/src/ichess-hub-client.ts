export interface IChessHubClient {
  // Returns to Group/Players (boardString: string)
  addBoardListener(): void;

  // Return Status to caller & optional Board to Group/Players
  sendMove(move: string): void;

  // Return possible Positions to Caller
  getPossiblePositions(row: number, column: number): void;
}
