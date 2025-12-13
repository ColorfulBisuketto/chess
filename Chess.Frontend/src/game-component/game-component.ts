import {Component, computed, effect, input, OnInit, signal} from '@angular/core';
import {BoardComponent} from "../board-component/board-component";
import {GameState, StateService, StatusCode} from '../state-service';
import {Player} from '../player';
import {Board} from '../board';
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';

import {MatMenuModule} from '@angular/material/menu';
import {MatButtonModule} from '@angular/material/button';
import {NgOptimizedImage} from '@angular/common';

@Component({
  selector: 'app-game-component',
  imports: [
    BoardComponent,
    ReactiveFormsModule,
    FormsModule,
    MatButtonModule,
    MatMenuModule,
    NgOptimizedImage
  ],
  templateUrl: './game-component.html',
  styleUrl: './game-component.css'
})
export class GameComponent implements OnInit {
  public gameId = input.required<string>();

  public currentPlayer = signal(Player.White);
  public statusVisible = signal(false);

  public state = computed(() => new StateService(this.gameId()));
  public board = computed<Board|null>(() => this.state().board());
  public turnName = computed(() => {
    return this.board()?.turn() == 0 ? "White" : "Black";
  });
  public isLoading = computed(() => this.state().connectionState() === StatusCode.Empty);
  public isConnectionError = computed(() => this.state().connectionState() === StatusCode.Error);
  public isEmpty = computed(() => this.board() === null);
  public isCheck = computed(() => this.state().gameState() == GameState.Check);
  public isCheckmate = computed(() => this.state().gameState() == GameState.Checkmate);
  public isDraw = computed(() => this.state().gameState() == GameState.Draw);

  protected readonly GameState = GameState;
  playerString: string = Player[Player.White];
  moveForm = new FormGroup({
    moveString: new FormControl('', [Validators.required, Validators.pattern(/^(([KQBNRP])?([a-z])?(\d+)?x?([a-z])(\d+)=?([QBNR])?[+#]?)|([0Oo]-[0Oo](-[0Oo])?)$/)])
  })

  constructor(private router: Router, private route: ActivatedRoute) {
    effect(() => {
      this.state().status();
      this.statusVisible.set(true);
      setTimeout(() => {this.statusVisible.set(false);}, 5000)
    });
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.playerString = params['player'];
      console.log(this.playerString);
      if (this.playerString === "White" || this.playerString === "Black") {
        switch (this.playerString) {
          case "White":
            this.currentPlayer.set(Player.White);
            break;
          case "Black":
            this.currentPlayer.set(Player.Black);
            break;
        }
      }
    });
  }

  displayColor(flip: boolean = false) {
    const player = this.board()?.turn();
    if (player === undefined) return "Red";
    const playerNumber = flip ? 1-player : player;
    return playerNumber == 0 ? "White" : "Black";
  }

  switchPlayer() {
    this.currentPlayer.update(value => (1 - value) as Player );
  }

  submitMove() {
    this.state().sendMove(<string>this.moveForm.value.moveString).then();
    this.moveForm.reset();
  }

  gotoGames() {
    this.state().stopConnection();
    this.router.navigate(['/games']).then();
  }

  undoMove() {
    this.state().undoMove().then();
  }

  async deleteGame() {
    await this.state().deleteGame();
    this.gotoGames()
  }
}
