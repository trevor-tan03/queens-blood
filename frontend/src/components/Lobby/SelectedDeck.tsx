import { useEffect, useState } from "react";
import { Card } from "../../types/Card";
import { isLegalDeck } from "../../utils/deckMethods";

const SelectedDeck = () => {
  const [selectedDeck, setSelectedDeck] = useState<Card[]>();

  useEffect(() => {
    const storedDeck = localStorage.getItem("deck");
    if (storedDeck) {
      const parsedDeck = JSON.parse(storedDeck) as Card[];
      if (isLegalDeck(parsedDeck))
        setSelectedDeck(parsedDeck);
    }
  }, []);

  return (
    <div className="grid grid-cols-3 xl:grid-cols-5 max-w-[50%]">
      {selectedDeck?.map((card, i) => (
        <img
          key={`card-${i}`}
          src={`../../../../assets/cards/${card.image}`}
          alt={card.name}
        />
      ))}
    </div>
  )
}

export default SelectedDeck