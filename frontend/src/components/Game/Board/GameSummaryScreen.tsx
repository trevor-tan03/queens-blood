import { useSignalR } from "../../../SignalR/SignalRProvider";
import ScoreCoin from "./ScoreCoin";

const GameSummaryScreen = () => {
  const { gameState, players, currPlayer } = useSignalR();
  const NUM_ROWS = 3;

  const getEndGameData = () => {
    let myTotalScore = 0;
    let enemyTotalScore = 0;
    let endGameMessage = "Draw";

    if (gameState?.laneScores !== undefined) {
      for (let i = 0; i < NUM_ROWS; i++) {
        const myScore = gameState?.laneScores[i];
        const enemyScore = gameState?.laneScores[i + 3];

        if (myScore > enemyScore)
          myTotalScore += myScore + gameState?.laneBonuses[i];
        else if (myScore < enemyScore)
          enemyTotalScore += enemyScore + gameState?.laneBonuses[i + 3];
      }

      if (myTotalScore > enemyTotalScore) endGameMessage = "Victory";
      else if (myTotalScore < enemyTotalScore) endGameMessage = "Defeat";
    }

    return [endGameMessage, myTotalScore, enemyTotalScore];
  };

  const data = getEndGameData();

  return (
    <div className="absolute w-full h-full grid place-items-center bg-black bg-opacity-60 z-[99]">
      <div className="w-full text-center p-3">
        <div className="flex gap-3 align-center justify-center">
          <div className="flex flex-col">
            <ScoreCoin
              value={data[1] as number}
              isMine={true}
              isMuted={false}
            />
            <div className="text-yellow-300 border border-yellow-300 bg-blue-900 mt-3">
              {currPlayer!.name}
            </div>
          </div>

          <div
            className={`text-4xl font-bold my-auto px-12 flex-1 
            ${data[0] === "Victory" ? "text-orange-300" : "text-gray-200"}`}
          >
            <span className="bg-gradient-to-r">{data[0]}</span>
          </div>

          <div className="flex flex-col">
            <ScoreCoin
              value={data[2] as number}
              isMine={false}
              isMuted={false}
            />
            <div className="text-yellow-300 border border-yellow-300 bg-red-900 mt-3">
              {players[(currPlayer!.playerIndex + 1) % 2].name}
            </div>
          </div>
        </div>
        <a href="/" className="bg-orange-300 p-3 px-8 rounded-full">
          Back
        </a>
      </div>
    </div>
  );
};

export default GameSummaryScreen;
