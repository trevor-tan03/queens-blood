import type { Tile } from "./Tile";

export type GameDTO = {
  laneScores: number[];
  board: Tile[];
};

export type Game = {
  laneScores: number[];
  board: Tile[][];
};
