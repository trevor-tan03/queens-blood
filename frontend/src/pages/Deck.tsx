import { useQuery } from "@tanstack/react-query";
import { useEffect, useState } from "react";
import { useSignalR } from "../SignalR/SignalRProvider";
import CardComponent from "../components/Card";
import { Card } from "../types/Card";
import { compressDeck, getCopiesLimit, getRemainingCopies, isLegalDeck, saveDeck } from "../utils/deckMethods";

const Deck = () => {
  const { gameCode } = useSignalR();
  const [deck, setDeck] = useState<Card[]>([]);

  useEffect(() => {
    const storedDeck = localStorage.getItem("deck");
    if (storedDeck) {
      const parsedDeck = JSON.parse(storedDeck) as Card[];
      if (isLegalDeck(parsedDeck))
        setDeck(parsedDeck);
    }
  }, []);

  const handleAdd = (card: Card) => {
    if (deck.length < 15) {
      const rarity = card.rarity;
      const copiesInDeck = deck.filter(c => c.id === card.id).length;

      // Player can only have one copy of a particular legendary card
      // Player can have at most two copies of a particular standard card
      if (
        (rarity === "Legendary" && copiesInDeck === 1) ||
        (rarity === "Standard" && copiesInDeck === 2)
      ) return;

      setDeck(d => [...d, card]);
    }
  }

  const handleRemove = (card: Card) => {
    if (deck.includes(card)) {
      const index = deck.findIndex(c => c.id === card.id);
      setDeck(d => (
        d.slice(0, index).concat(d.slice(index + 1))
      ))
    }
  }

  const api = `${import.meta.env.VITE_API_URL}/api/cards/base`;

  const { isPending, error, data } = useQuery<Card[]>({
    queryKey: ["baseCards"],
    queryFn: () => fetch(api).then((res) => res.json()),
    staleTime: 60000,
  });

  if (isPending) return "Loading...";

  if (error) return "An error occurred: " + error.message;

  return (
    <div className="p-8">
      <div>
        <span className={`${deck.length != 15 ? "text-red-600" : ""}`}>
          {deck.length}
        </span>
        /15
      </div>
      <div className="grid grid-cols-11 mb-6">
        {
          compressDeck(deck).map((c, i) => (
            <div key={`deck-${i}`} className="">
              <img
                src={`../../assets/cards/${c.card.image}`}
                alt={c.card.name}
                onClick={() => handleRemove(c.card)}
              />
              <div>{`${c.copies} / ${getCopiesLimit(c.card.rarity)}`}</div>
            </div>
          ))
        }
      </div>
      <button className="disabled:text-slate-500" disabled={!isLegalDeck(deck)} onClick={() => {
        if (isLegalDeck(deck)) {
          saveDeck(deck);
          history.back();
        }
      }}>Save</button>

      <div className="grid grid-cols-11 max-w-full gap-2">
        {data.map((d, i) => {
          const remCopies = getRemainingCopies(deck, d);

          if (remCopies) {
            return (
              <div key={`rem-${i}`}>
                <CardComponent
                  handleClick={handleAdd}
                  card={d}
                />
                <div>{`${remCopies}/${getCopiesLimit(d.rarity)}`}</div>
              </div>
            )
          }
        })}
      </div>
    </div>
  )
}

export default Deck