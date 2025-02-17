import { useSignalR } from "../SignalR/SignalRProvider";
import Board from "../components/Game/Board";
import Lobby from "./Lobby";

const Game = () => {
  const { gameStart, gameCode } = useSignalR();

  if (!gameCode) {
    window.location.replace("/");
  }

  return <>{gameStart ? <Board /> : <Lobby />}</>;
};

export default Game;
