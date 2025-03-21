import { BiEdit } from "react-icons/bi";
import { BsArrowLeft } from "react-icons/bs";
import { useNavigate } from "react-router";
import { useSignalR } from "../../SignalR/SignalRProvider";

const DeckHeader = () => {
  const { gameCode, leaveGame } = useSignalR();

  const navigate = useNavigate();

  const disconnect = async () => {
    await leaveGame(gameCode);
    window.location.replace("/");
  };

  return (
    <div className="flex w-full p-4 text-orange-300 bg-slate-600 border border-orange-300 border-b-0">
      <button className="text-2xl mr-2" onClick={disconnect}>
        <BsArrowLeft />
      </button>

      <h2 className="text-2xl tracking-wider">Deck</h2>

      <button
        className="flex gap-2 items-center ml-auto"
        onClick={() => {
          navigate(`/game/${gameCode}/deck`);
        }}
      >
        <BiEdit />
        Edit
      </button>
    </div>
  );
};

export default DeckHeader;
