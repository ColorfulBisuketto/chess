import { Component, computed, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { StatusCode } from '../state-service';

@Component({
  selector: 'app-game-list-component',
  imports: [
    RouterLink
  ],
  templateUrl: './game-list-component.html',
  styleUrl: './game-list-component.css'
})
export class GameListComponent {
  requestState = signal<StatusCode>(StatusCode.Empty);
  gameList = signal<number[]>([]);

  isLoading = computed(() => this.requestState() === StatusCode.Empty);
  isError = computed(() => this.requestState() === StatusCode.Error);
  isEmpty = computed(() => this.gameList().length === 0);

  constructor(private router: Router) {
    this.getGameList();
  }
  getGameList() {
    // TODO: Replace with actual HttpClient.
    fetch("http://localhost:5192/games")
      .then(async (response) => {
        this.gameList.set(await response.json())
        this.requestState.set(StatusCode.Success);
      })
      .catch((error) => {
        console.log(error);
        this.requestState.set(StatusCode.Error);
      });
  }

  gotoGame(gameId: number) {
    this.router.navigate([`/game/${gameId}`]).then();
  }
}
