import { useState } from "react";
import { BiSearch } from "react-icons/bi";
import type { Card } from "../../types/Card";
import { getCopiesLimit, getRemainingCopies } from "../../utils/deckMethods";
import CardComponent from "../Card";
import CardCopiesText from "./CardCopiesText";

interface Props {
  deck: Card[];
  setDeck: React.Dispatch<React.SetStateAction<Card[]>>;
  cardsList: Card[];
}

const CardList = ({ deck, setDeck, cardsList }: Props) => {
  const [shownCards, setShownCards] = useState(cardsList);
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

  const filterCardName = (cardName: string) => {
    setShownCards(cardsList.filter(card => card.name.toLowerCase().includes(cardName.toLowerCase())));
  }

  return (
    <div className="flex flex-col bg-slate-700 border border-orange-300 max-h-[500px] overflow-y-hidden">
      <div className="p-4 border-b border-orange-300 mb-2 flex justify-end gap-3 items-center">
        <h2 className="mr-auto text-2xl text-orange-300">Cards</h2>
        <button className="py-2 px-4 border border-orange-300 bg-orange-300 rounded-full">
          {"Card #"}
        </button>
        <button className="py-2 px-4 border border-orange-300 bg-orange-300 rounded-full">
          Ascending
        </button>
        <button className="py-2 px-4 border border-orange-300 rounded-full text-orange-300">
          Filters
        </button>
        <div className="flex items-center">
          <div className="h-[35px] flex-1 p-2 bg-slate-900 grid place-items-center rounded-l-md text-orange-300">
            <BiSearch />
          </div>
          <input
            onChange={(e) => filterCardName(e.target.value)}
            className="h-[35px] px-4"
            type="text"
            placeholder="Search card..."
          />
        </div>
      </div>

      <div className="grid grid-cols-4 md:grid-cols-6 lg:grid-cols-8 xl:grid-cols-11 max-w-full gap-2 p-6 flex-1 overflow-y-scroll">
        {shownCards.length > 0 ?
          shownCards.map((d, i) => {
            const remCopies = getRemainingCopies(deck, d);

            if (remCopies) {
              return (
                <div key={`rem-${i}`}>
                  <CardComponent
                    handleClick={handleAdd}
                    card={d}
                  />
                  <CardCopiesText>
                    {`${remCopies}/${getCopiesLimit(d.rarity)}`}
                  </CardCopiesText>
                </div>
              )
            }
          }) :
          <span className="text-slate-50">No results</span>}
      </div>
    </div>
  )
}

export default CardList