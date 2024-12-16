export type Card = {
  id: number;
  name: string;
  rank: number;
  power: number;
  rarity: "Standard" | "Legendary";
  ability: string;
  image: string;
};
