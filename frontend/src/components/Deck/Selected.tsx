import { useRef } from "react";
import type { Card } from "../../types/Card";
import { compressDeck, getCopiesLimit } from "../../utils/deckMethods";
import CardComponent from "../Card";
import CardCopiesText from "./CardCopiesText";

interface Props {
  deck: Card[];
  setDeck: React.Dispatch<React.SetStateAction<Card[]>>;
}

const Selected = ({ deck, setDeck }: Props) => {
  const containerRef = useRef<HTMLDivElement>(null);

  const handleRemove = (card: Card) => {
    if (deck.includes(card)) {
      const index = deck.findIndex(c => c.id === card.id);
      setDeck(d => (
        d.slice(0, index).concat(d.slice(index + 1))
      ))
    }
  }

  return (
    <div className="text-orange-300">
      <h1 className="text-3xl p-2">
        Card List
      </h1>
      <div className="bg-slate-700 bg-opacity-75 p-6 border-y border-orange-300">
        <div>
          <span className={`${deck.length != 15 ? "text-red-600" : ""}`}>
            {deck.length}
          </span>
          /15
        </div>
        <div className="flex mb-6 overflow-x-auto gap-2" ref={containerRef}>
          {
            compressDeck(deck).map((c, i) => (
              <div key={`deck-${i}`} className="">
                <CardComponent
                  card={c.card}
                  handleClick={() => handleRemove(c.card)}
                  containerRef={containerRef}
                  grow={false}
                />
                <CardCopiesText>
                  {`${c.copies} / ${getCopiesLimit(c.card.rarity)}`}
                </CardCopiesText>
              </div>
            ))
          }
        </div>
      </div>
    </div>
  )
}

export default Selected