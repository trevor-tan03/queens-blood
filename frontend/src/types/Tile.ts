import { CardDTO } from "./Card";

export type Tile = {
  ownerId?: string;
  bonusPower: number;
  card?: CardDTO;
  rank: number;
};
