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

  return (
    <div
      className={`w-[130px] h-[178px] relative grid place-items-center border-2
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
        <img
          className={`h-full ${isMine ? "" : "hue-rotate-[140deg]"}`}
          alt={tile.card?.name}
          src={`../../../../public/assets/cards/${tile.card?.image}`}
        />
      ) : tile.ownerId ? (
        <Pawn rank={tile.rank} isMine={isMine} />
      ) : null}
    </div>
  );
};

export default BoardTile;
