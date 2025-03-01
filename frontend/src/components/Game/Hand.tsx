import "../../../public/css/Card.css";
import type { Card as CardType } from "../../types/Card";
import { getRotation, getXOffset, getYOffset } from "../../utils/cardRotation";
import Draggable from "../dnd-kit/Draggable";
import DraggableContainer from "../dnd-kit/DraggableContainer";
import Card from "./Card";
import { useCardAbility } from "./CardAbilityContext";

interface HandProps {
  hand: CardType[];
}

const CardsInHand = ({ hand }: HandProps) => {
  const { setShownAbility } = useCardAbility();

  return (
    <div className="fixed left-0 bottom-0 h-64 w-screen">
      <div className="absolute w-full h-full left-[3rem] hand duration-200">
        <div className="relative w-full h-full">
          {hand.map((card, i) => (
            <DraggableContainer
              key={`hand-${i}`}
              rotate={getRotation(i, hand.length)}
              offsetX={getXOffset(i)}
              offsetY={getYOffset(i, hand.length)}
            >
              <Draggable id={`hand-${i}`} card={card}>
                <Card
                  card={card}
                  className=""
                  onMouseOver={() => setShownAbility(card.ability)}
                  onMouseLeave={() => setShownAbility("")}
                />
              </Draggable>
            </DraggableContainer>
          ))}
        </div>
      </div>
    </div>
  );
};

export default CardsInHand;
