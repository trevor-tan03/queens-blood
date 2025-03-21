import { useRef } from "react";
import { BsArrowLeft } from "react-icons/bs";
import { TbCardsFilled } from "react-icons/tb";
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
      const index = deck.findIndex((c) => c.id === card.id);
      setDeck((d) => d.slice(0, index).concat(d.slice(index + 1)));
    }
  };

  return (
    <div className="text-orange-300">
      <div className="flex ml-3 items-center py-2">
        <button className="text-2xl" onClick={() => history.back()}>
          <BsArrowLeft />
        </button>
        <h1 className="text-3xl p-2 mr-3">Card List</h1>
        <div className="px-4 py-2 border rounded-full border-orange-300">
          <div
            className={`${
              deck.length != 15 ? "text-red-600" : ""
            } flex gap-2 items-center`}
          >
            <TbCardsFilled size="24" />
            <div>
              {deck.length}
              /15
            </div>
          </div>
        </div>
      </div>
      <div className="bg-slate-700 bg-opacity-75 p-6 border-y border-orange-300">
        <div className="flex mb-6 overflow-x-auto gap-2" ref={containerRef}>
          {compressDeck(deck).map((c, i) => (
            <div key={`deck-${i}`}>
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
          ))}
        </div>
      </div>
    </div>
  );
};

export default Selected;
