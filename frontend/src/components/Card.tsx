import { useRef } from "react";
import type { Card } from "../types/Card";

interface Props {
  handleClick: (card: Card) => void;
  card: Card;
  containerRef: React.RefObject<HTMLDivElement>;
  grow?: boolean;
}

const CardComponent = ({
  handleClick,
  card,
  containerRef,
  grow = true,
}: Props) => {
  const popupRef = useRef<HTMLDivElement>(null);
  const cardRef = useRef<HTMLDivElement>(null);
  const POPUP_WIDTH = 300;
  const { name, image, ability } = card;

  const getPosition = (popupEl: HTMLDivElement) => {
    if (!cardRef.current) return "R";

    const container = containerRef.current!.getBoundingClientRect();
    const boundaries = popupEl.getBoundingClientRect();

    if (boundaries.left - POPUP_WIDTH <= container.left) {
      popupEl.classList.add(`right-[-${POPUP_WIDTH}px]`);
    } else {
      popupEl.classList.add(`left-[-${POPUP_WIDTH}px]`);
    }
  };

  return (
    <div
      className={`relative ${grow ? "" : "min-w-20 max-w-24"}`}
      onClick={() => handleClick(card)}
      ref={cardRef}
      onMouseOver={() => {
        popupRef.current!.style.display = "block";
        getPosition(popupRef.current!);
      }}
      onMouseOut={() => {
        popupRef.current!.style.display = "none";
        popupRef.current!.classList.remove(`left-[-${POPUP_WIDTH}px]`);
        popupRef.current!.classList.remove(`right-[-${POPUP_WIDTH}px]`);
      }}
    >
      <img src={`../../assets/cards/${image}`} alt={name} loading="lazy" />
      <div
        className={`z-50 absolute top-0 w-[${POPUP_WIDTH}px] min-h-[120px] bg-slate-800 border border-orange-300 hidden p-4 text-white bg-opacity-90 pointer-events-none`}
        ref={popupRef}
      >
        <h3 className="text-center text-orange-300 text-xl border-b border-orange-300 mb-1">
          {name}
        </h3>
        {ability}
      </div>
    </div>
  );
};

export default CardComponent;
