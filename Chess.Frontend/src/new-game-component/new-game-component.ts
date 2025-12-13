import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { Player } from '../player';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-new-game-component',
  imports: [
    RouterLink,
    ReactiveFormsModule
  ],
  templateUrl: './new-game-component.html',
  styleUrl: './new-game-component.css'
})
export class NewGameComponent {
  constructor(private router: Router) { }

  newGameForm = new FormGroup({
    player: new FormControl(null, [Validators.required]),
  });

  createNewGame() {
    fetch("http://localhost:5192/newGame")
      .then(async (response) => {
        const gameId = await response.json();
        const player = this.newGameForm.value.player;
        if (player === null || player === undefined) {
          this.router.navigate(['/game', gameId]).then();
        } else {
          this.router.navigate(['/game', gameId], { queryParams: { player: Player[player] } }).then();
        }
      })
      .catch((error) => {
        console.log(error);
      });
  }

  protected readonly Player = Player;

  onSubmit() {
    const player = this.newGameForm.value.player;
    console.log(player);
    if (player !== null && player !== undefined) {
      console.log(Player[player]);
    }
  }
}
