import type { Tile } from "../../../types/Tile";

interface Props {
  tile: Tile;
}

const BoardTile = ({ tile }: Props) => {
  return (
    <div className="w-[170px] h-[200px]">
      {tile.card ? (
        <img
          className="h-full"
          alt={tile.card?.name}
          src={`../../../../public/assets/cards/${tile.card?.image}`}
        />
      ) : (
        tile.rank
      )}
    </div>
  );
};

export default BoardTile;
