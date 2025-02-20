import { useDraggable } from "@dnd-kit/core";
import React from "react";

const Draggable = ({
  children,
  id,
}: {
  children: React.ReactNode;
  id: string;
}) => {
  const { attributes, listeners, setNodeRef, transform } = useDraggable({
    id: id,
  });
  const style = transform
    ? {
        transform: `translate3d(${transform.x}px, ${transform.y}px, 0)`,
      }
    : undefined;

  return (
    <button
      ref={setNodeRef}
      style={style}
      {...listeners}
      {...attributes}
      className="relative"
    >
      {children}
    </button>
  );
};

export default Draggable;
