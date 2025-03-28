import { useSignalR } from "../../../SignalR/SignalRProvider";

const PlayerNames = () => {
  const { currPlayer, players, playing } = useSignalR();

  return (
    <div className="w-dvw h-dvh absolute flex place-items-center z-50 translate-y-48 pointer-events-none">
      <div className="flex-1">
        <div
          className={`bg-blue-900 w-48 py-6 text-yellow-300 rounded-r-full border-yellow-300 border-2 text-center text-xl ${
            playing === currPlayer!.id ? "brightness-50" : ""
          }`}
        >
          {currPlayer!.name}
        </div>
      </div>

      <div className="flex-1">
        <div
          className={`bg-red-900 w-48 py-6 text-yellow-300 ml-auto rounded-l-full border-yellow-300 border-2 text-center text-xl ${
            playing !== currPlayer!.id ? "brightness-50" : ""
          }`}
        >
          {players[(currPlayer!.playerIndex + 1) % 2].name}
        </div>
      </div>
    </div>
  );
};

export default PlayerNames;
