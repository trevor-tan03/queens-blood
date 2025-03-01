import { useSignalR } from "../../../SignalR/SignalRProvider";
import type { Tile } from "../../../types/Tile";
import Pawn from "./Pawn";

interface Props {
  tile: Tile;
}

const BoardTile = ({ tile }: Props) => {
  const { currPlayer } = useSignalR();
  const isMine = currPlayer !== undefined && currPlayer.id === tile?.ownerId;

  return (
    <div
      className={`w-[130px] h-[178px] grid place-items-center border-4 ${
        isMine ? "border-green-300" : "border-red-300"
      }`}
    >
      {tile.card ? (
        <img
          className="h-full"
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
