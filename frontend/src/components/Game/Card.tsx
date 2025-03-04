import { useSignalR } from "../../SignalR/SignalRProvider";
import type { Card as CardType } from "../../types/Card";

interface Props {
  card: CardType;
  className?: string;
  onClick?: () => void;
  onMouseOver?: () => void;
  onMouseLeave?: () => void;
  children?: React.ReactNode;
}

const Card = ({
  card,
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
      onClick={onClick}
      onMouseOver={onMouseOver}
      onMouseLeave={onMouseLeave}
    >
      {children}
      <img
        className={`h-full hover:scale-125 transition-transform duration-200 rounded-lg ${
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
