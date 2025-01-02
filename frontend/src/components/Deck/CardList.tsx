import { useRef } from "react";
import { BiSearch } from "react-icons/bi";
import { GrAscend, GrDescend, GrFilter } from "react-icons/gr";
import type { Card } from "../../types/Card";
import { getCopiesLimit, getRemainingCopies } from "../../utils/deckMethods";
import CardComponent from "../Card";
import { useOverlay } from "../Overlay";
import CardCopiesText from "./CardCopiesText";
import { useFilters } from "./FilterProvider";

interface Props {
  deck: Card[];
  setDeck: React.Dispatch<React.SetStateAction<Card[]>>;
  cardsList: Card[];
}



const CardList = ({ deck, setDeck }: Props) => {
  const { toggleOrder, order, shownCards, filterCardName } = useFilters();

  const { showOverlay } = useOverlay();

  const cardListRef = useRef<HTMLDivElement>(null);

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

  return (
    <div className="flex flex-col bg-slate-700 border border-orange-300 max-h-[500px] overflow-y-hidden bg-opacity-60 h-[70rem]">
      <div className="p-4 border-b border-orange-300 mb-2 flex justify-end gap-3 items-center flex-wrap bg-slate-800">
        <h2 className="mr-auto text-2xl text-orange-300">Cards</h2>

        <button
          className="py-2 px-4 border border-orange-300 bg-orange-300 rounded-full hidden sm:block"
          onClick={toggleOrder}>
          {order === "asc" ? "Ascending" : "Descending"}
        </button>
        <button
          className="py-2 px-4 border border-orange-300 bg-orange-300 rounded-full sm:hidden"
          onClick={toggleOrder}>
          {order === "asc" ? <GrAscend /> : <GrDescend />}
        </button>

        <button className="py-2 px-4 border border-orange-300 rounded-full text-orange-300 sm:hidden" onClick={showOverlay}>
          <GrFilter />
        </button>

        <button className="py-2 px-4 border border-orange-300 rounded-full text-orange-300 hidden sm:block" onClick={showOverlay}>
          Filters
        </button>

        <div className="flex items-center">
          <div className="h-[35px] flex-1 p-2 bg-slate-900 grid place-items-center rounded-l-md text-orange-300">
            <BiSearch />
          </div>
          <input
            onChange={(e) => filterCardName(e.target.value)}
            className="h-[35px] px-4 w-full"
            type="text"
            placeholder="Search card..."
          />
        </div>
      </div>

      <div
        className="grid grid-cols-4 md:grid-cols-6 lg:grid-cols-8 xl:grid-cols-11 max-w-full gap-2 p-6 flex-1 overflow-y-scroll"
        ref={cardListRef}
      >
        {shownCards.length > 0 ?
          shownCards.map((d, i) => {
            const remCopies = getRemainingCopies(deck, d);

            if (remCopies) {
              return (
                <div key={`rem-${i}`}>
                  <CardComponent
                    containerRef={cardListRef}
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