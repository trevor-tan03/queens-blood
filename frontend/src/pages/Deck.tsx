import { useQuery } from "@tanstack/react-query";
import { useState } from "react";
import { useNavigate } from "react-router";
import { useSignalR } from "../SignalR/SignalRProvider";
import CardComponent from "../components/Card";
import { Card } from "../types/Card";

interface CompressedCard {
  [key: string]: {
    copies: number;
    card: Card;
  }
}

const Deck = () => {
  const { gameCode } = useSignalR();
  const navigate = useNavigate();
  const [deck, setDeck] = useState<Card[]>([]);

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

  const compressDeck = (deck: Card[]) => {
    const compressedDeck: CompressedCard = {};

    deck.forEach((c) => {
      const key = `${c.id}`;
      if (compressedDeck[key]) {
        compressedDeck[key].copies += 1;
      } else {
        compressedDeck[key] = { card: c, copies: 1 }
      }
    })

    return Object.values(compressedDeck)
  }

  const getRemainingCopies = (card: Card) => {
    const rarity = card.rarity;
    const copiesInDeck = deck.filter(c => c.id === card.id).length;

    if (rarity === "Standard") {
      return 2 - copiesInDeck;
    }

    return 1 - copiesInDeck;
  }

  const getCopiesLimit = (rarity: Card["rarity"]) => {
    return rarity === "Standard" ? 2 : 1;
  }

  const api = `${import.meta.env.VITE_API_URL}/api/Card`;

  const { isPending, error, data } = useQuery<Card[]>({
    queryKey: ["baseCards"],
    queryFn: () => fetch(api).then((res) => res.json()),
    staleTime: 60000,
  });

  if (isPending) return "Loading...";

  if (error) return "An error occurred: " + error.message;

  return (
    <div className="p-8">
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
      <button onClick={() => {
        navigate(`/game/${gameCode}`);
      }}>Save</button>

      <div className="grid grid-cols-11 max-w-full gap-2">
        {data.map((d, i) => {
          const remCopies = getRemainingCopies(d);

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