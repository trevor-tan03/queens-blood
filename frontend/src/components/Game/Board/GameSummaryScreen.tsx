import { useSignalR } from "../../../SignalR/SignalRProvider";
import ScoreCoin from "./ScoreCoin";

const GameSummaryScreen = () => {
  const { gameState } = useSignalR();
  const NUM_ROWS = 3;

  const getEndGameData = () => {
    let myTotalScore = 0;
    let enemyTotalScore = 0;
    let endGameMessage = "DRAW!";

    if (gameState?.laneScores !== undefined) {
      for (let i = 0; i < NUM_ROWS; i++) {
        const myScore = gameState?.laneScores[i];
        const enemyScore = gameState?.laneScores[i + 3];

        if (myScore > enemyScore)
          myTotalScore += myScore + gameState?.laneBonuses[i];
        else if (myScore < enemyScore)
          enemyTotalScore += enemyScore + gameState?.laneBonuses[i + 3];
      }

      if (myTotalScore > enemyTotalScore) endGameMessage = "YOU WIN!";
      else if (myTotalScore < enemyTotalScore) endGameMessage = "YOU LOSE!";
    }

    return [endGameMessage, myTotalScore, enemyTotalScore];
  };

  const data = getEndGameData();

  return (
    <div className="absolute w-full h-full grid place-items-center bg-black bg-opacity-60 z-50">
      <div className="w-full text-center p-3">
        <div className="flex gap-3 align-center justify-center">
          <ScoreCoin value={data[1] as number} isMine={true} />
          <span className="text-2xl font-bold text-white my-auto px-12">
            {data[0]}
          </span>
          <ScoreCoin value={data[2] as number} isMine={false} />
        </div>
        <a href="/" className="bg-orange-300 p-3 px-8 rounded-full">
          Back
        </a>
      </div>
    </div>
  );
};

export default GameSummaryScreen;
