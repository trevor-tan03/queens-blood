import "../../../../public/css/Card.css";
import type { Card } from "../../../types/Card";
import {
  getRotation,
  getXOffset,
  getYOffset,
} from "../../../utils/cardRotation";
import { getTimeFormat } from "../../../utils/formatDate";
import { useCardAbility } from "../CardAbilityContext";
import MulliganCard from "./MulliganCard";

interface Props {
  cards: Card[];
  mulliganCardAtIndex: (cardIndex: number) => void;
  isSelectedToMulligan: (index: number) => boolean;
  hasConfirmedMulligan: boolean;
  confirmMulligan: () => void;
  deadline: Date;
}

const MulliganCardsScreen = ({
  cards,
  mulliganCardAtIndex,
  isSelectedToMulligan,
  hasConfirmedMulligan,
  confirmMulligan,
  deadline,
}: Props) => {
  const { shownAbility } = useCardAbility();

  return (
    <div className="absolute top-0 h-screen w-screen bg-black bg-opacity-40 text-slate-100">
      <h1 className="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 text-3xl">
        Select cards to mulligan
      </h1>

      <div className="flex absolute bottom-[16rem] left-[3rem]">
        {cards.map((card, index) => {
          const offsetX = getXOffset(index);
          const offsetY = getYOffset(index, cards.length);
          const rotate = getRotation(index, cards.length);

          return (
            <MulliganCard
              key={`card-${index}`}
              card={card}
              offsetX={offsetX}
              offsetY={offsetY}
              rotate={rotate}
              isSelectedToMulligan={isSelectedToMulligan(index)}
              handleClick={() => mulliganCardAtIndex(index)}
            />
          );
        })}
      </div>

      <div className="absolute bottom-[1rem] left-[1rem] text-2xl">
        {shownAbility}
      </div>

      {!hasConfirmedMulligan && (
        <button
          onClick={confirmMulligan}
          className="absolute bottom-[8rem] right-0"
        >
          Confirm
        </button>
      )}
      <div>{getTimeFormat(deadline)}</div>
    </div>
  );
};

export default MulliganCardsScreen;
