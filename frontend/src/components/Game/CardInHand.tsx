import type { Card } from "../../types/Card";

interface CardProps {
  card: Card;
  rotate: number;
  offsetX: number;
  offsetY: number;
}

const CardInHand = ({ card, rotate, offsetX, offsetY }: CardProps) => {
  return (
    <div
      className="absolute hover:z-[99] hover:cursor-pointer drop-shadow-sm"
      style={{
        transform: `translate(${offsetX}px, ${offsetY}px) rotateZ(${rotate}deg)`,
      }}
    >
      <img
        className="w-fit h-fit hover:scale-125 transition-transform duration-200 max-h-[200px]"
        draggable={false}
        src={`../../../../assets/cards/${card.image}`}
        alt={card.name}
      />
    </div>
  );
};

export default CardInHand;
