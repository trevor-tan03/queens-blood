import { useSignalR } from "../../../SignalR/SignalRProvider";
import Droppable from "../../dnd-kit/Droppable";
import BoardTile from "./BoardTile";
import ScoreBonus from "./ScoreBonus";
import ScoreCoin from "./ScoreCoin";

const Board = () => {
  const { gameState, gameStatePreview } = useSignalR();
  const NUM_ROWS = 3;

  if (!gameState) return null;
  const { board, laneScores, laneBonuses } = gameStatePreview ?? gameState;
  console.log(laneBonuses);

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
              <BoardTile tile={tile} bgColour={(row + col) % 2 ? "b" : "w"} />
            </Droppable>
          ));
        })}

        {laneScores.map((score, index) => {
          const row = index % NUM_ROWS;
          const enemyRow = (index + 3) % laneScores.length;

          return (
            <div
              key={`score-${index}`}
              className="my-auto w-[150px] grid relative"
              style={{
                gridColumnStart: index < 3 ? 1 : 7,
                gridRowStart: row + 1,
              }}
            >
              <ScoreCoin value={score} isMine={index < 3} />

              {laneBonuses[index] > 0 && (
                <ScoreBonus
                  isWinningLane={laneScores[index] > laneScores[enemyRow]}
                  isMine={index < 3}
                  bonusPoints={laneBonuses[index]}
                />
              )}
            </div>
          );
        })}
      </div>
    );
  };

  return renderBoard();
};

export default Board;
