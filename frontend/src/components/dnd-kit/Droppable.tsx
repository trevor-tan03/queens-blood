import { useDroppable } from "@dnd-kit/core";
import { useEffect, useState } from "react";
import { useSignalR } from "../../SignalR/SignalRProvider";
import type { Tile } from "../../types/Tile";
import { useCompatibleTiles } from "../Game/Board/LegalTilesContext";

const Droppable = ({
  children,
  tile,
  id,
  row,
  col,
}: {
  children: React.ReactNode;
  tile: Tile;
  id: string;
  row: number;
  col: number;
}) => {
  const { setNodeRef } = useDroppable({
    id: id,
  });
  const { currPlayer } = useSignalR();
  const { compatibleTiles } = useCompatibleTiles();
  const [isOccupied, setIsOccupied] = useState(tile?.card !== null);
  const [isMine, setIsMine] = useState(
    currPlayer && currPlayer?.id === tile?.ownerId
  );

  useEffect(() => {
    setIsOccupied(tile?.card !== null);
    setIsMine(currPlayer && currPlayer?.id === tile?.ownerId);
  }, [tile?.ownerId]);

  return (
    <div
      ref={isMine && !isOccupied ? setNodeRef : null}
      style={{ gridColumnStart: col + 2, gridRowStart: row + 1 }}
      className={`border w-full ${
        compatibleTiles.includes(id) ? "bg-yellow-300 bg-opacity-50" : ""
      }`}
    >
      {children}
    </div>
  );
};

export default Droppable;
