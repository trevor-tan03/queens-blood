import { useSignalR } from "../SignalR/SignalRProvider";
import BoardContext from "../components/Game/Board/BoardContext";
import GameScreen from "../components/Game/Board/GameScreen";
import GameSummaryScreen from "../components/Game/Board/GameSummaryScreen";
import { CardAbilityProvider } from "../components/Game/CardAbilityContext";
import MulliganPhase from "../components/Game/Mulligan/MulliganPhase";
import Lobby from "./Lobby";

const Game = () => {
  const { gameStart, gameCode, mulliganPhaseEnded, isGameOver } = useSignalR();

  if (!gameCode) window.location.replace("/");
  if (!gameStart) return <Lobby />;

  return (
    <BoardContext>
      <CardAbilityProvider>
        <div className="relative h-dvh w-dvw bg-slate-700">
          {isGameOver && <GameSummaryScreen />}
          {!mulliganPhaseEnded ? <MulliganPhase /> : <GameScreen />}
          {/* <Background /> */}
        </div>
      </CardAbilityProvider>
    </BoardContext>
  );
};

export default Game;
