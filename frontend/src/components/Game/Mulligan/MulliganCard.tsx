import type { Card as CardType } from "../../../types/Card";
import Card from "../../Game/Card";
import { useCardAbility } from "../CardAbilityContext";

interface Props {
  card: CardType;
  offsetX: number;
  offsetY: number;
  rotate: number;
  isSelectedToMulligan: boolean;
  handleClick: () => void;
}

const MulliganCard = ({
  card,
  offsetX,
  offsetY,
  rotate,
  isSelectedToMulligan,
  handleClick,
}: Props) => {
  const { setShownAbility } = useCardAbility();

  return (
    <Card
      card={card}
      rotate={rotate}
      offsetX={offsetX}
      offsetY={offsetY}
      className="card"
      onClick={handleClick}
      onMouseOver={() => setShownAbility(card.ability)}
      onMouseLeave={() => setShownAbility("")}
    >
      {isSelectedToMulligan ? (
        <div className="absolute top-0 right-0 z-[99] w-full h-full bg-black opacity-80 pointer-events-none card-overlay"></div>
      ) : null}
    </Card>
  );
};

export default MulliganCard;
