import type { Card } from "../types/Card";

export type CompressedCard = {
  [key: string]: {
    copies: number;
    card: Card;
  };
};

export const compressDeck = (deck: Card[]) => {
  const compressedDeck: CompressedCard = {};

  deck.forEach((c) => {
    const key = `${c.id}`;
    if (compressedDeck[key]) {
      compressedDeck[key].copies += 1;
    } else {
      compressedDeck[key] = { card: c, copies: 1 };
    }
  });

  return Object.values(compressedDeck);
};

export const getRemainingCopies = (deck: Card[], card: Card) => {
  const rarity = card.rarity;
  const copiesInDeck = deck.filter((c) => c.id === card.id).length;

  if (rarity === "Standard") {
    return 2 - copiesInDeck;
  }

  return 1 - copiesInDeck;
};

export const getCopiesLimit = (rarity: Card["rarity"]) => {
  return rarity === "Standard" ? 2 : 1;
};

export const isLegalDeck = (deck: Card[]) => {
  if (deck.length !== 15) return false;

  let legal = true;
  const compressedDeck = compressDeck(deck);

  Object.values(compressedDeck).map(({ copies, card }) => {
    if (
      (card.rarity === "Standard" && copies > 2) ||
      (card.rarity === "Legendary" && copies > 1)
    ) {
      legal = false;
    }
  });

  return legal;
};

export const saveDeck = (deck: Card[]) => {
  localStorage.setItem("deck", JSON.stringify(deck));
  dispatchEvent(new Event("storage"));
};

export const getDefaultDeck = () => {
  /**
   * ID  | Name
   * ----------------------
   * 1   | Security Officer
   * 2   | Riot Trooper
   * 7   | Levrikon
   * 8   | Grasslands Wolf
   * 11  | Elphadunk
   * 12  | Cactuar
   * 13  | Crystalline Crab
   * 98  | Titan
   * 107 | Chocobo & Moogle
   */
  return [1, 1, 2, 7, 7, 8, 8, 11, 11, 12, 12, 13, 13, 98, 107];
};
