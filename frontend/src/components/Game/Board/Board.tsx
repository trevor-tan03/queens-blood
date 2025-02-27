import { useSignalR } from "../../../SignalR/SignalRProvider";
import Droppable from "../../dnd-kit/Droppable";
import BoardTile from "./BoardTile";

const Board = () => {
  const { gameState, gameStatePreview } = useSignalR();
  if (!gameState) return null;
  const { board } = gameStatePreview ?? gameState;

  const renderBoard = () => {
    return (
      <div className="grid grid-cols-5 min-w-fit">
        {board.map((_, row) => {
          return board[row].map((tile, col) => (
            <Droppable key={`${row},${col}`} id={`${row},${col}`} tile={tile}>
              <BoardTile tile={tile} />
            </Droppable>
          ));
        })}
      </div>
    );
  };

  return renderBoard();
};

export default Board;
