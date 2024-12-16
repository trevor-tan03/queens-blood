import { useRef } from 'react';
import type { Card } from '../types/Card';

interface Props {
  handleClick: (card: Card) => void;
  card: Card
}

const CardComponent = ({ handleClick, card }: Props) => {
  const cardRef = useRef<HTMLDivElement>(null);
  const { name, image, ability } = card;

  return (
    <div
      className="relative"
      onClick={() => handleClick(card)}
      onMouseOver={() => {
        cardRef.current!.style.display = "block";
      }}
      onMouseOut={() => {
        cardRef.current!.style.display = "none";
      }}
    >
      <img
        src={`../../assets/cards/${image}`}
        alt={name} loading="lazy"
      />
      <div className="z-50 absolute top-0 bg-slate-300 border border-black hidden" ref={cardRef}>
        {ability}
      </div>
    </div>
  )
}

export default CardComponent