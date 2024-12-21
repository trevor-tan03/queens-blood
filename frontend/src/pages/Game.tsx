import { BiEdit } from "react-icons/bi";
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
        <div className="flex *:flex-1 gap-4 bg-slate-700 p-6 h-dvh overflow-y-visible md:overflow-y-hidden md:grid-cols-1 flex-col-reverse md:flex-row">
          <div className="overflow-y-hidden">
            <div className="flex justify-between w-full p-2 text-orange-300">
              <h1 className="text-2xl">Deck</h1>
              <button
                className="flex gap-2 items-center"
                onClick={() => {
                  navigate(`/game/${gameCode}/deck`)
                }}>
                Edit
                <BiEdit />
              </button>
            </div>
            <SelectedDeck />
          </div>
          <PlayersSection />
        </div>
      }
    </>
  )
}

export default Game