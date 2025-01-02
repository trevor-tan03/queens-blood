import { useEffect } from "react";
import { useNavigate } from "react-router";
import { useSignalR } from "../../SignalR/SignalRProvider";
import { useOverlay } from "../Overlay";

interface Props {
  name: string;
  setName: React.Dispatch<React.SetStateAction<string>>;
}

const HomeMenu = ({ name, setName }: Props) => {
  const { showOverlay } = useOverlay();
  const { gameCode, createGame } = useSignalR();
  const navigate = useNavigate();

  useEffect(() => {
    if (gameCode) {
      navigate(`/game/${gameCode}`);
    }
  }, [gameCode, navigate]);

  return (
    <div className="flex flex-col span px-6 bg-slate-700 bg-opacity-75 items-center w-full max-w-96 *:w-full gap-3 py-16 border border-orange-300 absolute sm:px-16">
      <h1 className="text-4xl font-bold text-center text-orange-300 mb-8">
        {"Queen's Blood"}
      </h1>
      <input
        id="name"
        className="border border-slate-500 p-2 px-6 rounded-full mb-4"
        placeholder="Enter your name"
        type="text"
        value={name}
        onChange={((e) => setName(e.target.value))}
      />

      <button
        className="text-slate-700 bg-orange-300 p-2 rounded-full"
        onClick={async () => await createGame(name)}
      >
        Create
      </button>
      <button
        onClick={showOverlay}
        className="text-slate-700 bg-orange-300 p-2 rounded-full"
      >
        Join
      </button>
      <button
        className="text-orange-300 border border-orange-300 p-2 rounded-full"
        onClick={() => navigate("/deck")}
      >
        Change Deck
      </button>
    </div>
  )
}

export default HomeMenu