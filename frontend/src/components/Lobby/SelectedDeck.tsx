import { useEffect, useRef, useState } from "react";
import { Card } from "../../types/Card";
import { isLegalDeck } from "../../utils/deckMethods";
import CardComponent from "../Card";

const SelectedDeck = () => {
  const [selectedDeck, setSelectedDeck] = useState<Card[]>();
  const containerRef = useRef<HTMLDivElement>(null);

  const setDeck = () => {
    const storedDeck = localStorage.getItem("deck");

    if (storedDeck) {
      const parsedDeck = JSON.parse(storedDeck) as Card[];
      if (isLegalDeck(parsedDeck)) setSelectedDeck(parsedDeck);
    }
  };

  useEffect(() => {
    setDeck();
    window.addEventListener("storage", setDeck);

    return () => {
      window.removeEventListener("storage", setDeck);
    };
  }, []);

  return (
    <div
      className="grid grid-cols-3 h-full xl:grid-cols-5 xl:h-auto overflow-y-auto p-12 bg-slate-800 bg-opacity-65 border border-orange-300 max-w-[800px]"
      ref={containerRef}
    >
      {selectedDeck?.map((card, i) => (
        <CardComponent
          key={`card-${i}`}
          card={card}
          containerRef={containerRef}
          handleClick={() => {}}
          clickAction={null}
        />
      ))}
    </div>
  );
};

export default SelectedDeck;
