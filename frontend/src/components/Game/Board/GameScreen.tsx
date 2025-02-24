import { DndContext, DragEndEvent } from "@dnd-kit/core";
import { useSignalR } from "../../../SignalR/SignalRProvider";
import { useCardAbility } from "../CardAbilityContext";
import CardsInHand from "../Hand";
import Board from "./Board";

const GameScreen = () => {
  const { gameCode, hand, playCard } = useSignalR();
  const { shownAbility } = useCardAbility();

  const handleDragEnd = (event: DragEndEvent) => {
    const { active, over } = event;

    if (over) {
      const cardId = parseInt(active.id.toString().slice(5));
      const [row, col] = over.id.toString().split(",");
      playCard(gameCode, cardId, parseInt(row), parseInt(col));
    }
  };

  return (
    <DndContext onDragEnd={(event) => handleDragEnd(event)}>
      <div>
        <div className="relative">
          <CardsInHand hand={hand} />
        </div>

        <Board />

        <div className="absolute bottom-[1rem] left-[1rem] text-2xl">
          {shownAbility}
        </div>
      </div>
    </DndContext>
  );
};

export default GameScreen;
