import { useEffect } from "react";
import { useSignalR } from "../../../SignalR/SignalRProvider";
import Droppable from "../../dnd-kit/Droppable";
import BoardTile from "./BoardTile";

const Board = () => {
  const { gameState, gameStatePreview } = useSignalR();

  useEffect(() => {
    console.log(gameStatePreview);
  }, [gameStatePreview]);

  // REMOVE WHEN DONE!!!
  useEffect(() => {
    console.log(gameState?.laneScores);
  }, [gameState?.laneScores]);

  if (!gameState) return null;
  const { board, laneScores } = gameStatePreview ?? gameState;

  const renderBoard = () => {
    return (
      <div
        className="grid min-w-fit rotate-x-20"
        style={{
          gridTemplateColumns: "repeat(7, auto)",
        }}
      >
        {board.map((_, row) => {
          return board[row].map((tile, col) => (
            <Droppable
              key={`${row},${col}`}
              id={`${row},${col}`}
              tile={tile}
              row={row}
              col={col}
            >
              <BoardTile tile={tile} />
            </Droppable>
          ));
        })}

        {laneScores.map((score, index) => {
          const row = index % 3;
          return (
            <div
              key={`score-${index}`}
              className="my-auto"
              style={{
                gridColumnStart: index < 3 ? 1 : 7,
                gridRowStart: row + 1,
              }}
            >
              {score}
            </div>
          );
        })}
      </div>
    );
  };

  return renderBoard();
};

export default Board;
