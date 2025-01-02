import { useRef } from 'react';
import type { Card } from '../types/Card';

interface Props {
  handleClick: (card: Card) => void;
  card: Card,
  containerRef: React.RefObject<HTMLDivElement>,
  grow?: boolean
}

const CardComponent = ({ handleClick, card, containerRef, grow = true }: Props) => {
  const popupRef = useRef<HTMLDivElement>(null);
  const cardRef = useRef<HTMLDivElement>(null);
  const { name, image, ability } = card;

  const getPosition = (popupEl: HTMLDivElement) => {
    if (!cardRef.current) return "R"

    const padding = 30;
    const container = containerRef.current!.getBoundingClientRect();
    const boundaries = popupEl.getBoundingClientRect();

    if (boundaries.right >= 0.75 * (container.width - padding)) {
      popupEl.classList.add("left-[-200px]");
    } else {
      popupEl.classList.add("right-[-200px]");
    }
  }

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
        popupRef.current!.classList.remove("left-[-200px]");
        popupRef.current!.classList.remove("right-[-200px]");
      }}
    >
      <img
        src={`../../assets/cards/${image}`}
        alt={name} loading="lazy"
      />
      <div
        className={`z-50 absolute top-0 w-[200px] min-h-[120px] bg-slate-800 border border-orange-300 hidden p-2 text-orange-300`}
        ref={popupRef}>
        {ability}
      </div>
    </div>
  )
}

export default CardComponent