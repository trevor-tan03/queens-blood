import "../../../public/css/Card.css";
import type { Card as CardType } from "../../types/Card";
import { getRotation, getXOffset, getYOffset } from "../../utils/cardRotation";
import Card from "./Card";
import { useCardAbility } from "./CardAbilityContext";

interface HandProps {
  hand: CardType[];
}

const CardsInHand = ({ hand }: HandProps) => {
  const { setShownAbility } = useCardAbility();

  return (
    <div className="absolute top-0 h-screen w-screen overflow-y-hidden">
      <div className="flex absolute bottom-[-6rem] left-[3rem] hand transition-transform duration-200">
        {hand.map((card, i) => (
          <Card
            card={card}
            rotate={getRotation(i, hand.length)}
            offsetX={getXOffset(i)}
            offsetY={getYOffset(i, hand.length)}
            className=""
            onMouseOver={() => setShownAbility(card.ability)}
            onMouseLeave={() => setShownAbility("")}
            key={`hand-${i}`}
          />
        ))}
      </div>
    </div>
  );
};

export default CardsInHand;
