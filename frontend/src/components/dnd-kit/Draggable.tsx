import { useDraggable } from "@dnd-kit/core";
import React from "react";
import { useSignalR } from "../../SignalR/SignalRProvider";

const Draggable = ({
  children,
  id,
}: {
  children: React.ReactNode;
  id: string;
}) => {
  const { currPlayer, playing } = useSignalR();
  const { attributes, listeners, setNodeRef, transform } = useDraggable({
    id: id,
  });
  const style =
    transform && currPlayer?.id == playing
      ? {
          transform: `translate3d(${transform.x}px, ${transform.y}px, 0)`,
        }
      : undefined;

  return (
    <div
      ref={currPlayer?.id == playing ? setNodeRef : null}
      style={style}
      {...listeners}
      {...attributes}
      className="relative"
    >
      {children}
    </div>
  );
};

export default Draggable;
