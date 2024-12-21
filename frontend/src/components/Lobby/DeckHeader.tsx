import { BiEdit } from "react-icons/bi";
import { useNavigate } from "react-router";
import { useSignalR } from "../../SignalR/SignalRProvider";

const DeckHeader = () => {
  const { gameCode } = useSignalR();

  const navigate = useNavigate();

  return (
    <div className="flex justify-between w-full p-4 text-orange-300 bg-slate-600 border border-orange-300 border-b-0">
      <h2 className="text-2xl tracking-wider">Deck</h2>
      <button
        className="flex gap-2 items-center"
        onClick={() => {
          navigate(`/game/${gameCode}/deck`)
        }}>
        <BiEdit />
        Edit
      </button>
    </div>
  )
}

export default DeckHeader