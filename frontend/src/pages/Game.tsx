import { useSignalR } from "../SignalR/SignalRProvider";
import GameSummaryScreen from "../components/Game/Banner/GameSummaryScreen";
import BoardContext from "../components/Game/Board/BoardContext";
import GameScreen from "../components/Game/Board/GameScreen";
import { CardAbilityProvider } from "../components/Game/CardAbilityContext";
import MulliganPhase from "../components/Game/Mulligan/MulliganPhase";
import Background from "../components/Home/Background";
import Lobby from "./Lobby";

const Game = () => {
  const { gameStart, gameCode, mulliganPhaseEnded, isGameOver } = useSignalR();

  if (!gameCode) window.location.replace("/");
  if (!gameStart) return <Lobby />;

  return (
    <BoardContext>
      <CardAbilityProvider>
        <div className="relative h-dvh w-dvw">
          {isGameOver && <GameSummaryScreen />}
          {!mulliganPhaseEnded ? <MulliganPhase /> : <GameScreen />}
          <Background />
        </div>
      </CardAbilityProvider>
    </BoardContext>
  );
};

export default Game;
