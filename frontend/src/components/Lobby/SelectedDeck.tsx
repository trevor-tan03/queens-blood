import { useEffect, useRef, useState } from "react";
import { Card } from "../../types/Card";
import { isLegalDeck } from "../../utils/deckMethods";
import CardComponent from "../Card";

const SelectedDeck = () => {
  const [selectedDeck, setSelectedDeck] = useState<Card[]>();
  const containerRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const storedDeck = localStorage.getItem("deck");
    if (storedDeck) {
      const parsedDeck = JSON.parse(storedDeck) as Card[];
      if (isLegalDeck(parsedDeck))
        setSelectedDeck(parsedDeck);
    }
  }, []);

  return (
    <div
      className="grid grid-cols-3 h-full xl:grid-cols-5 xl:h-auto overflow-y-auto p-12 bg-slate-800 border border-orange-300 max-w-screen-lg"
      ref={containerRef}
    >
      {selectedDeck?.map((card, i) => (
        <CardComponent
          key={`card-${i}`}
          card={card}
          containerRef={containerRef}
          handleClick={() => { }}
        />
      ))}
    </div>
  )
}

export default SelectedDeck