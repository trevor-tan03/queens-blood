import { useQuery } from "@tanstack/react-query";
import { useState } from "react";
import { useNavigate } from "react-router";
import { useSignalR } from "../SignalR/SignalRProvider";
import CardComponent from "../components/Card";
import { Card } from "../types/Card";

const Deck = () => {
  const { gameCode } = useSignalR();
  const navigate = useNavigate();
  const [deck, setDeck] = useState<Card[]>([]);

  const handleAdd = (card: Card) => {
    if (deck.length < 15) {
      const rarity = card.rarity;

      // Player can only have one copy of a particular legendary card
      if (rarity === "Legendary" && deck.find(c => c.id === card.id)) {
        return;
      }
      // Player can have at most two copies of a particular standard card
      else if (rarity === "Standard" && deck.filter(c => c.id === card.id).length === 2) {
        return;
      }

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

  const hasRemainingCopies = (card: Card) => {
    const rarity = card.rarity;
    const copiesInDeck = deck.filter(c => c.id === card.id).length;

    if (rarity === "Standard") {
      return copiesInDeck < 2;
    }

    return copiesInDeck < 1;
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
        {deck.map(card => (
          <div className="">
            <img src={`../../public/assets/cards/${card.image}`} alt={card.name} onClick={() => handleRemove(card)} />
          </div>
        ))}
      </div>
      <button onClick={() => {
        navigate(`/game/${gameCode}`);
      }}>Save</button>

      <div className="grid grid-cols-11 max-w-full gap-2">
        {data.map(d => {
          if (hasRemainingCopies(d)) {
            return (
              <div>
                <CardComponent
                  handleClick={handleAdd}
                  card={d}
                />
                <div>{d.rarity === "Standard" ? "2/2" : "1/1"}</div>
              </div>
            )
          }
        })}
      </div>
    </div>
  )
}

export default Deck