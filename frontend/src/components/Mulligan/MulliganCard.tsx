import type { Card } from "../../types/Card";

interface Props {
  card: Card;
  offsetX: number;
  offsetY: number;
  rotate: number;
  isSelectedToMulligan: boolean;
  handleClick: () => void;
  setShownAbility: React.Dispatch<React.SetStateAction<string>>;
}

const MulliganCard = ({
  card,
  offsetX,
  offsetY,
  rotate,
  isSelectedToMulligan,
  handleClick,
  setShownAbility,
}: Props) => {
  return (
    <div
      onClick={handleClick}
      className="relative hover:cursor-pointer border-r-red-400 card"
      style={{
        transform: `translate(${offsetX}px, ${offsetY}px) rotateZ(${rotate}deg)`,
      }}
      onMouseOver={() => setShownAbility(card.ability)}
      onMouseLeave={() => setShownAbility("")}
    >
      {isSelectedToMulligan ? (
        <div className="absolute top-0 right-0 z-[99] w-full h-full bg-black opacity-80 pointer-events-none card-overlay"></div>
      ) : null}
      <img
        className="w-fit transition-transform duration-200 max-h-[200px] hover:scale-125"
        draggable={false}
        src={`../../../../assets/cards/${card.image}`}
        alt={card.name}
      />
    </div>
  );
};

export default MulliganCard;
