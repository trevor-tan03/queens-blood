import { useNavigate } from "react-router";
import { useSignalR } from "../SignalR/SignalRProvider";
import Board from "../components/Game/Board";
import PlayersSection from "../components/Lobby/PlayersSection";
import SelectedDeck from "../components/Lobby/SelectedDeck";

const Game = () => {
  const { gameCode, gameStart } = useSignalR();

  const navigate = useNavigate();
  return (
    <>
      {gameStart
        ?
        <Board />
        :
        <div className="flex">
          <div>
            <button onClick={() => {
              navigate(`/game/${gameCode}/deck`)
            }}>Decks</button>
            <SelectedDeck />
          </div>
          <PlayersSection />
        </div>
      }
    </>
  )
}

export default Game