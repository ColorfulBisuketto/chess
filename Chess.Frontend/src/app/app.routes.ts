import { Routes } from '@angular/router';
import {GameComponent} from '../game-component/game-component';
import {GameListComponent} from '../game-list-component/game-list-component';
import {NewGameComponent} from '../new-game-component/new-game-component';
import {NotFoundComponent} from '../not-found-component/not-found-component';

export const routes: Routes = [
  { path: '', component: GameListComponent },
  { path: 'games', component: GameListComponent },
  { path: 'game/new', component: NewGameComponent },
  { path: 'game/:gameId', component: GameComponent },
  { path: '**', component: NotFoundComponent },
];
