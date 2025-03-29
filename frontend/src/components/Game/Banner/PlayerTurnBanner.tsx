import { useSignalR } from "../../../SignalR/SignalRProvider";

const PlayerTurnBanner = () => {
  const { playing, currPlayer } = useSignalR();

  return (
    <div className="absolute w-dvw h-dvh grid place-items-center z-[99999] pointer-events-none">
      <div
        className={`w-full h-24 bg-opacity-95 grid place-items-center text-yellow-300 text-2xl translate-y-8 border-y-2 border-yellow-200
        ${
          playing === currPlayer!.id
            ? "bg-blue-900"
            : playing === null
            ? "hidden"
            : "bg-red-900"
        } animate-fadeOut`}
      >
        {playing === currPlayer!.id ? "Your Turn" : "Opponent's Turn"}
      </div>
    </div>
  );
};

export default PlayerTurnBanner;
