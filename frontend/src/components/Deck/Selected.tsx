import type { Card } from "../../types/Card";
import { compressDeck, getCopiesLimit } from "../../utils/deckMethods";
import CardCopiesText from "./CardCopiesText";

interface Props {
  deck: Card[];
  setDeck: React.Dispatch<React.SetStateAction<Card[]>>;
}

const Selected = ({ deck, setDeck }: Props) => {
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
        <div className="flex mb-6 whitespace-nowrap overflow-x-auto gap-2">
          {
            compressDeck(deck).map((c, i) => (
              <div key={`deck-${i}`} className="">
                <img
                  className="max-w-24"
                  src={`../../assets/cards/${c.card.image}`}
                  alt={c.card.name}
                  onClick={() => handleRemove(c.card)}
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