import { useDraggable } from "@dnd-kit/core";
import React, { useEffect } from "react";
import { useSignalR } from "../../SignalR/SignalRProvider";
import type { Card } from "../../types/Card";
import { useCompatibleTiles } from "../Game/Board/LegalTilesContext";

const Draggable = ({
  children,
  id,
  card,
}: {
  children: React.ReactNode;
  id: string;
  card: Card;
}) => {
  const { currPlayer, playing } = useSignalR();
  const { getCompatibleTiles, resetCompatibleTiles } = useCompatibleTiles();
  const { attributes, listeners, setNodeRef, transform, isDragging } =
    useDraggable({
      id: id,
    });

  const style =
    transform && currPlayer?.id == playing
      ? {
          transform: `translate3d(${transform.x}px, ${transform.y}px, 0)`,
        }
      : undefined;

  useEffect(() => {
    if (isDragging) {
      // Highlight the tiles that this card can be placed on
      getCompatibleTiles(card.rank);
    } else {
      resetCompatibleTiles();
    }
  }, [isDragging]);

  return (
    <div
      ref={currPlayer?.id == playing ? setNodeRef : null}
      style={style}
      {...listeners}
      {...attributes}
      className="absolute w-[140px] h-full z-50"
    >
      {children}
    </div>
  );
};

export default Draggable;
