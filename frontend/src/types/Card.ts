export type Card = {
  id: number;
  name: string;
  rank: number;
  power: number;
  rarity: "Standard" | "Legendary";
  ability: string;
  image: string;
  condition: string;
  action: Ability | null;
  target?: string;
  value?: number;
};

export enum Ability {
  Enhance = "+P",
  Enfeeble = "-P",
  AddToHand = "add",
  Destroy = "destroy",
  IncreaseRank = "+R",
  Spawn = "spawn",
  ScoreBonus = "+Score",
}
