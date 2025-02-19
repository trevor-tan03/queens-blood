import { useSignalR } from "../SignalR/SignalRProvider";
import GameBoard from "../components/Game/Board/GameBoard";
import { CardAbilityProvider } from "../components/Game/CardAbilityContext";
import MulliganPhase from "../components/Game/Mulligan/MulliganPhase";
import Lobby from "./Lobby";

const Game = () => {
  const { gameStart, gameCode, mulliganPhaseEnded } = useSignalR();

  if (!gameCode) window.location.replace("/");
  if (!gameStart) return <Lobby />;

  return (
    <CardAbilityProvider>
      {!mulliganPhaseEnded ? <MulliganPhase /> : <GameBoard />}
    </CardAbilityProvider>
  );
};

export default Game;
