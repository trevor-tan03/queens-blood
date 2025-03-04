export type Card = {
  id: number;
  name: string;
  rank: number;
  power: number;
  rarity: "Standard" | "Legendary";
  image: string;
  condition: string;
  ability: string;
  action: Action | null;
};

export enum Action {
  Enhance = "+P",
  Enfeeble = "-P",
  AddToHand = "add",
  Destroy = "destroy",
  IncreaseRank = "+R",
  Spawn = "spawn",
  ScoreBonus = "+Score",
}

export type CardDTO = {
  name: string;
  image: string;
  ability: string;
};
