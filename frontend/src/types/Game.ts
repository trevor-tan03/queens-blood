import type { Tile } from "./Tile";

export type GameDTO = {
  laneScores: number[];
  laneBonuses: number[];
  board: Tile[];
};

export interface Game extends Omit<GameDTO, "board"> {
  board: Tile[][];
}
