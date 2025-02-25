import { useDroppable } from "@dnd-kit/core";
import { useSignalR } from "../../SignalR/SignalRProvider";
import type { Tile } from "../../types/Tile";

const Droppable = ({
  children,
  tile,
  id,
}: {
  children: React.ReactNode;
  tile: Tile;
  id: string;
}) => {
  const { isOver, setNodeRef } = useDroppable({
    id: id,
  });

  const { currPlayer } = useSignalR();

  const style = {
    borderColor: isOver ? "green" : "red",
  };

  return (
    <div
      ref={currPlayer && currPlayer?.id === tile?.ownerId ? setNodeRef : null}
      className="border w-fit"
      style={style}
    >
      {children}
    </div>
  );
};

export default Droppable;
