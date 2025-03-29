import { useRef } from "react";
import { useSignalR } from "../../../SignalR/SignalRProvider";
import type { Tile } from "../../../types/Tile";
import Pawn from "./Pawn";

interface Props {
  tile: Tile;
  bgColour: "b" | "w";
}

const BoardTile = ({ tile, bgColour }: Props) => {
  const { currPlayer } = useSignalR();
  const isMine = currPlayer !== undefined && currPlayer.id === tile?.ownerId;
  const tileRef = useRef<HTMLDivElement>(null);
  const popupRef = useRef<HTMLDivElement>(null);
  const POPUP_WIDTH = 300;

  return (
    <div
      ref={tileRef}
      onMouseOver={() => {
        popupRef.current!.style.display = "block";
      }}
      onMouseOut={() => {
        popupRef.current!.style.display = "none";
        popupRef.current!.style.left = "auto";
        popupRef.current!.style.right = "auto";
      }}
      className={`w-[135px] h-[145px] relative grid place-items-center border-2
        ${isMine ? "border-green-300" : "border-red-300"}
        ${
          bgColour === "b"
            ? "bg-violet-950 bg-opacity-30"
            : "bg-slate-100 bg-opacity-50"
        }`}
    >
      {tile.bonusPower !== 0 && (
        <div
          className={`absolute z-50 bg-opacity-80 bottom-0 h-16 w-full grid place-items-center font-bold ${
            tile.bonusPower < 0 ? "bg-red-500" : "bg-green-500"
          }`}
        >
          {tile.bonusPower}
        </div>
      )}

      {tile.card ? (
        <div>
          <div
            className={`z-[99] absolute top-0 min-h-[120px] bg-slate-800 border border-orange-300 hidden p-4 text-white bg-opacity-90 pointer-events-none ml-[135px]`}
            style={{
              minWidth: `${POPUP_WIDTH}px`,
              maxWidth: `${POPUP_WIDTH}px`,
            }}
            ref={popupRef}
          >
            <h3 className="text-center text-orange-300 text-xl border-b border-orange-300 mb-1">
              {tile.card?.name}
            </h3>
            {tile?.card.ability}
          </div>
          <img
            className={`w-[135px] h-[145px] ${
              isMine ? "" : "hue-rotate-[140deg]"
            }`}
            alt={tile.card?.name}
            src={`../../../../public/assets/cards/${tile.card?.image}`}
          />
        </div>
      ) : tile.ownerId ? (
        <Pawn rank={tile.rank} isMine={isMine} />
      ) : null}
    </div>
  );
};

export default BoardTile;
