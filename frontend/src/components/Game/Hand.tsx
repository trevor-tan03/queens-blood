import type { Card } from "../../types/Card";
import { getRotation, getXOffset, getYOffset } from "../../utils/cardRotation";
import CardInHand from "./CardInHand";

interface HandProps {
  hand: Card[];
  cardsToMulligan: number[];
}

const CardsInHand = ({ hand }: HandProps) => {
  return (
    <div className="relative">
      {hand.map((card, i) => (
        <CardInHand
          card={card}
          rotate={getRotation(i, hand.length)}
          offsetX={getXOffset(i)}
          offsetY={getYOffset(i, hand.length)}
          key={`hand-${i}`}
        />
      ))}
    </div>
  );
};

export default CardsInHand;
