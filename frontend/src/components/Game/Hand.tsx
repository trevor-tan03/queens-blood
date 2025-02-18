import type { Card as CardType } from "../../types/Card";
import { getRotation, getXOffset, getYOffset } from "../../utils/cardRotation";
import Card from "./Card";

interface HandProps {
  hand: CardType[];
}

const CardsInHand = ({ hand }: HandProps) => {
  return (
    <div className="absolute top-0 h-screen w-screen">
      <div className="flex absolute bottom-[4rem] left-[3rem]">
        {hand.map((card, i) => (
          <Card
            card={card}
            rotate={getRotation(i, hand.length)}
            offsetX={getXOffset(i)}
            offsetY={getYOffset(i, hand.length)}
            className=""
            key={`hand-${i}`}
          />
        ))}
      </div>
    </div>
  );
};

export default CardsInHand;
