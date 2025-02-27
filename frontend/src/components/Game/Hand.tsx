import "../../../public/css/Card.css";
import type { Card as CardType } from "../../types/Card";
import { getRotation, getXOffset, getYOffset } from "../../utils/cardRotation";
import Draggable from "../dnd-kit/Draggable";
import Card from "./Card";
import { useCardAbility } from "./CardAbilityContext";

interface HandProps {
  hand: CardType[];
}

const CardsInHand = ({ hand }: HandProps) => {
  const { setShownAbility } = useCardAbility();

  return (
    <div className="fixed left-0 top-0 h-screen w-screen overflow-y-hidden">
      <div className="flex fixed bottom-[-6rem] left-[3rem] hand transition-all duration-200">
        {hand.map((card, i) => (
          <Draggable key={`hand-${i}`} id={`hand-${i}`} card={card}>
            <Card
              card={card}
              rotate={getRotation(i, hand.length)}
              offsetX={getXOffset(i)}
              offsetY={getYOffset(i, hand.length)}
              className=""
              onMouseOver={() => setShownAbility(card.ability)}
              onMouseLeave={() => setShownAbility("")}
            />
          </Draggable>
        ))}
      </div>
    </div>
  );
};

export default CardsInHand;
