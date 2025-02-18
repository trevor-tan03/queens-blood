import type { Card as CardType } from "../../types/Card";

interface Props {
  card: CardType;
  offsetX: number;
  offsetY: number;
  rotate: number;
  className?: string;
  onClick?: () => void;
  onMouseOver?: () => void;
  onMouseLeave?: () => void;
  children?: React.ReactNode;
}

const Card = ({
  card,
  offsetX,
  offsetY,
  rotate,
  className,
  children,
  onClick,
  onMouseOver,
  onMouseLeave,
}: Props) => {
  return (
    <div
      className={`hover:cursor-pointer ${className}`}
      style={{
        transform: `translate(${offsetX}px, ${offsetY}px) rotateZ(${rotate}deg)`,
      }}
      onClick={onClick}
      onMouseOver={onMouseOver}
      onMouseLeave={onMouseLeave}
    >
      {children}
      <img
        className="w-fit h-fit hover:scale-125 transition-transform duration-200 max-h-[200px]"
        draggable={false}
        src={`../../../../assets/cards/${card.image}`}
        alt={card.name}
      />
    </div>
  );
};

export default Card;
