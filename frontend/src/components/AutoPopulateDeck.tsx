import { useQuery } from "@tanstack/react-query";
import { useEffect } from "react";
import type { Card } from "../types/Card";
import { getDefaultDeck, saveDeck } from "../utils/deckMethods";

const AutoPopulateDeck = () => {
  const api = `${
    import.meta.env.VITE_API_URL || process.env.VITE_API_URL
  }/api/cards/base`;

  const { data } = useQuery<Card[]>({
    queryKey: ["baseCards"],
    queryFn: () => fetch(api).then((res) => res.json()),
    staleTime: 60000,
  });

  useEffect(() => {
    const deck = localStorage.getItem("deck");

    if (!deck && data) {
      const defaultDeck = getDefaultDeck();
      const cards: Card[] = [];
      defaultDeck.forEach((index) => cards.push(data[index - 1]));
      saveDeck(cards);
    }
  }, [data]);

  return null;
};

export default AutoPopulateDeck;
