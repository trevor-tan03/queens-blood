import { useEffect, useRef } from "react";
import { useSignalR } from "../../../SignalR/SignalRProvider";

const PlayerTurnBanner = () => {
  const { playing, currPlayer } = useSignalR();
  const bannerRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    setTimeout(() => {
      if (bannerRef.current !== null) {
        bannerRef.current.classList.add("animate-fadeOut");
      }
    }, 1500);
  }, [playing]);

  return (
    <div className="absolute w-dvw h-dvh grid place-items-center z-[50]">
      <div
        ref={bannerRef}
        className={`w-full h-24 bg-opacity-95 grid place-items-center text-yellow-300 text-2xl translate-y-8 border-y-2 border-yellow-200
        ${playing === currPlayer!.id ? "bg-blue-900" : "bg-red-900"}`}
      >
        {playing === currPlayer!.id ? "Your Turn" : "Opponent's Turn"}
      </div>
    </div>
  );
};

export default PlayerTurnBanner;
