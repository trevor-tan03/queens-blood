import { DndContext, DragEndEvent, DragOverEvent } from "@dnd-kit/core";
import { useEffect } from "react";
import { useSignalR } from "../../../SignalR/SignalRProvider";
import { useCardAbility } from "../CardAbilityContext";
import CardsInHand from "../Hand";
import Board from "./Board";

const GameScreen = () => {
  const {
    gameCode,
    hand,
    playCard,
    playing,
    currPlayer,
    skipTurn,
    previewPlay,
    cancelPreview,
  } = useSignalR();

  const { shownAbility } = useCardAbility();

  useEffect(() => {
    document.addEventListener("mouseup", () => cancelPreview());
    return () => document.removeEventListener("mouseup", () => cancelPreview());
  }, [cancelPreview]);

  const handleDragEnd = (event: DragEndEvent) => {
    const { active, over } = event;

    if (over) {
      const cardId = parseInt(active.id.toString().slice(5));
      const [row, col] = over.id.toString().split(",");
      playCard(gameCode, cardId, parseInt(row), parseInt(col));
    }
  };

  const handleDragOver = (event: DragOverEvent) => {
    const { active, over } = event;

    if (over) {
      const cardId = parseInt(active.id.toString().slice(5));
      const [row, col] = over.id.toString().split(",");
      previewPlay(gameCode, cardId, parseInt(row), parseInt(col));
    }
  };

  return (
    <DndContext
      onDragEnd={(event) => handleDragEnd(event)}
      onDragOver={(event) => handleDragOver(event)}
      onDragCancel={() => {
        cancelPreview();
      }}
    >
      <div>
        <div className="relative">
          <CardsInHand hand={hand} />
        </div>

        <Board />

        {currPlayer?.id === playing && (
          <button
            className="z-50 cursor-pointer absolute right-0 bottom-[6rem]"
            onClick={() => skipTurn(gameCode)}
          >
            Pass
          </button>
        )}

        <div className="absolute bottom-[1rem] left-[1rem] text-2xl pointer-events-none">
          {shownAbility}
        </div>
      </div>
    </DndContext>
  );
};

export default GameScreen;
