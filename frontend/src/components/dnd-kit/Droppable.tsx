import { useDroppable } from "@dnd-kit/core";

const Droppable = ({
  children,
  id,
}: {
  children: React.ReactNode;
  id: string;
}) => {
  const { isOver, setNodeRef } = useDroppable({
    id: id,
  });

  const style = {
    borderColor: isOver ? "green" : "red",
  };

  return (
    <div ref={setNodeRef} className="border w-fit" style={style}>
      {children}
    </div>
  );
};

export default Droppable;
