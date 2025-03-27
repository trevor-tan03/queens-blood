import { useSignalR } from "../../../SignalR/SignalRProvider";

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
      <div className="bg-red-300 w-full text-center p-3">
        {data[1]}
        {data[0]}
        {data[2]}
      </div>
    </div>
  );
};

export default GameSummaryScreen;
