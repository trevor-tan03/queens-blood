import { createContext, useContext, useEffect, useState } from "react";
import { Card, type Action } from "../../types/Card";

interface IFilters {
  rank: number[];
  rarity: ("Standard" | "Legendary")[];
  ability: Action[];
}

interface IFilterContext {
  filters: IFilters;
  shownCards: Card[];
  order: "asc" | "desc";
  setFilters: React.Dispatch<React.SetStateAction<IFilters>>;
  toggleOrder: () => void;
  filterCardName: (name: string) => void;
  applyFilters: () => void;
}

const FilterContext = createContext<IFilterContext | undefined>(undefined);

interface Props {
  children: React.ReactNode;
  cardsList: Card[];
}

const FilterProvider = ({ children, cardsList }: Props) => {
  const [shownCards, setShownCards] = useState<Card[]>(cardsList);
  // filteredCards - helps maintain filters when the search is being cleared
  const [filteredCards, setFilteredCards] = useState<Card[]>(cardsList);
  const [search, setSearch] = useState("");
  const [order, setOrder] = useState<"asc" | "desc">("asc");
  const [filters, setFilters] = useState<IFilters>({
    rank: [],
    rarity: [],
    ability: [],
  });

  /**
   * Toggle functions
   */
  const toggleOrder = () => setOrder((o) => (o === "asc" ? "desc" : "asc"));
  useEffect(() => {
    setShownCards((cards) => cards.slice().reverse());
  }, [order]);

  /**
   * Search functions
   */
  const filterCardName = (cardName: string) => {
    setShownCards(
      filteredCards.filter((c) =>
        c.name.toLowerCase().includes(cardName.toLowerCase())
      )
    );
    setSearch(cardName);
  };

  /**
   * Apply filters
   */
  const applyFilters = () => {
    let cardsToShow = [...cardsList];

    if (
      !filters.rank.length &&
      !filters.rarity.length &&
      !filters.ability.length
    ) {
      setFilteredCards(cardsList);
      setShownCards(
        cardsList.filter((c) =>
          c.name.toLowerCase().includes(search.toLowerCase())
        )
      );
      return;
    }

    // Apply rank filter
    cardsToShow = cardsToShow.filter((c) =>
      filters.rank.length ? filters.rank.includes(c.rank) : true
    );

    // Apply rarity filter
    cardsToShow = cardsToShow.filter((c) =>
      filters.rarity.length ? filters.rarity.includes(c.rarity) : true
    );

    // Apply ability filter
    cardsToShow = cardsToShow.filter((c) => {
      if (filters.ability.length && !c.action) return false;
      else if (!c.action) return true;
      return filters.ability.length ? filters.ability.includes(c.action) : true;
    });

    setFilteredCards(cardsToShow);
    setShownCards(
      cardsToShow.filter((c) =>
        c.name.toLowerCase().includes(search.toLowerCase())
      )
    );
  };

  return (
    <FilterContext.Provider
      value={{
        filters,
        shownCards,
        order,
        setFilters,
        toggleOrder,
        filterCardName,
        applyFilters,
      }}
    >
      {children}
    </FilterContext.Provider>
  );
};

export default FilterProvider;

export const useFilters = () => {
  const context = useContext(FilterContext);
  if (!context)
    throw new Error("FilterContext must be used within a FilterProvider");
  return context;
};
