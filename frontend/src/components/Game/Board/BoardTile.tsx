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
    <div className={`w-[170px] h-[200px] grid place-items-center`}>
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
