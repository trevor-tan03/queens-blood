import { useRef } from "react";
import type { Card } from "../types/Card";

interface Props {
  handleClick: (card: Card) => void;
  card: Card;
  containerRef: React.RefObject<HTMLDivElement>;
  clickAction: "add" | "remove" | null;
  grow?: boolean;
}

const CardComponent = ({
  handleClick,
  card,
  containerRef,
  clickAction,
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
      popupEl.style.right = `-${POPUP_WIDTH}px`;
    } else {
      popupEl.style.left = `-${POPUP_WIDTH}px`;
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
        popupRef.current!.style.left = "auto";
        popupRef.current!.style.right = "auto";
      }}
    >
      <div className="relative">
        {clickAction && (
          <div
            className="absolute h-full w-full grid place-items-center opacity-0 hover:opacity-100 text-xl text-white transition-all duration-500"
            style={
              clickAction === "add"
                ? {
                    background:
                      "radial-gradient(rgba(0,255,0,0.8), rgba(0,255,0,0.2))",
                  }
                : {
                    background:
                      "radial-gradient(rgba(255,0,50,0.8), rgba(255,0,0,0.2))",
                  }
            }
          >
            <div className="shadow-2xl">
              {clickAction === "add" ? "Add" : "Remove"}
            </div>
          </div>
        )}
        <img src={`../../assets/cards/${image}`} alt={name} loading="lazy" />
      </div>
      <div
        className={`z-50 absolute top-0 min-h-[120px] bg-slate-800 border border-orange-300 hidden p-4 text-white bg-opacity-90 pointer-events-none`}
        style={{ minWidth: `${POPUP_WIDTH}px`, maxWidth: `${POPUP_WIDTH}px` }}
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
