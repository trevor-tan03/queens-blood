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
  isSelectedToMulligan,
  handleClick,
  offsetX,
  offsetY,
  rotate,
}: Props) => {
  const { setShownAbility } = useCardAbility();

  return (
    <div
      className="origin-center absolute w-[140px]"
      style={{
        transform: `translate(${offsetX}px, ${offsetY}px) rotateZ(${rotate}deg)`,
      }}
    >
      <Card
        card={card}
        className="card relative"
        onClick={handleClick}
        onMouseOver={() => setShownAbility(card.ability)}
        onMouseLeave={() => setShownAbility("")}
      >
        {isSelectedToMulligan ? (
          <div className="absolute top-0 right-0 z-[99] w-full h-full bg-black opacity-80 pointer-events-none card-overlay"></div>
        ) : null}
      </Card>
    </div>
  );
};

export default MulliganCard;
