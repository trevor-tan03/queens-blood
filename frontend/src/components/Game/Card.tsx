import { useSignalR } from "../../SignalR/SignalRProvider";
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
  const { currPlayer, playing, mulliganPhaseEnded } = useSignalR();
  const isCurrentPlayer = () => currPlayer?.id === playing;

  return (
    <div
      className={`pointer-events-auto ${className}`}
      style={{
        transform: `translate(${offsetX}px, ${offsetY}px) rotateZ(${rotate}deg)`,
      }}
      onClick={onClick}
      onMouseOver={onMouseOver}
      onMouseLeave={onMouseLeave}
    >
      {children}
      <img
        className={`h-[170px] hover:scale-125 transition-transform duration-200 max-h-[200px] rounded-lg ${
          isCurrentPlayer() && mulliganPhaseEnded
            ? "ring-4 ring-blue-600 ring-opacity-20 shadow-blue-500 shadow-[0px_0px_20px]"
            : ""
        }`}
        src={`../../../../assets/cards/${card.image}`}
        alt={card.name}
        draggable={false}
      />
    </div>
  );
};

export default Card;
