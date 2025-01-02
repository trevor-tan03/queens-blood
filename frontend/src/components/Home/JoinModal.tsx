import { useState } from "react";
import { useSignalR } from "../../SignalR/SignalRProvider";
import { useOverlay } from "../Overlay";

interface Props {
  name: string;
}

const JoinModal = ({ name }: Props) => {
  const { joinGame } = useSignalR();
  const [code, setCode] = useState('');
  const { hideOverlay } = useOverlay();

  const handleOverlayClick = (e: React.MouseEvent<HTMLDivElement>) => {
    e.stopPropagation();
    hideOverlay();
  };

  const handleModalClick = (e: React.MouseEvent<HTMLDivElement>) => {
    e.stopPropagation();
  };

  return (
    <div
      className="absolute h-dvh w-full bg-black bg-opacity-45 grid place-items-center z-50"
      onClick={handleOverlayClick}
    >
      <div
        className="bg-slate-600 max-w-96 flex flex-col gap-2 p-16 z-[99]"
        onClick={handleModalClick}
      >
        <h2 className="text-3xl text-center text-orange-300">Join Game</h2>
        <input
          id="code"
          placeholder="Enter Code"
          className="border border-slate-500 py-2 px-4 rounded-full"
          type="text"
          value={code}
          onChange={(e) => setCode(e.target.value)}
        />
        <button
          className="text-slate-700 bg-orange-300 p-2 rounded-full"
          onClick={async () => await joinGame(code, name)}
        >
          Join
        </button>
      </div>
    </div>
  );
}

export default JoinModal;
