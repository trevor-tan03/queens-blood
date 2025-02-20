import { DndContext, DragEndEvent } from "@dnd-kit/core";
import { useSignalR } from "../../../SignalR/SignalRProvider";
import Droppable from "../../dnd-kit/Droppable";
import { useCardAbility } from "../CardAbilityContext";
import CardsInHand from "../Hand";
import { useBoardContext } from "./BoardContext";
import BoardTile from "./BoardTile";

const GameScreen = () => {
  const { hand } = useSignalR();
  const { shownAbility } = useCardAbility();
  const { setChild } = useBoardContext();

  const handleDragEnd = (event: DragEndEvent) => {
    const { active, over } = event;
    console.log(active);
    console.log(over);

    if (over) {
      setChild(active.id);
    }
  };

  return (
    <DndContext onDragEnd={(event) => handleDragEnd(event)}>
      <div>
        <div className="relative">
          <CardsInHand hand={hand} />
        </div>

        <Droppable id="droppable">
          <BoardTile />
        </Droppable>

        <div className="absolute bottom-[1rem] left-[1rem] text-2xl">
          {shownAbility}
        </div>
      </div>
    </DndContext>
  );
};

export default GameScreen;
