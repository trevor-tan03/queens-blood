import { BiClipboard } from "react-icons/bi";
import { MdCheck } from "react-icons/md";
import { useSignalR } from "../../SignalR/SignalRProvider";
import { useAlertContext } from "../AlertContext";
import Chat from "./Chat";
import Waiting from "./Waiting";

const PlayersSection = () => {
  const { gameCode, players, readyUp, unready, currPlayer } = useSignalR();
  const { createAlert } = useAlertContext();

  const isPlayerReady = (connectionId: string) => {
    return players.find((player) => player.id === connectionId)?.isReady;
  };

  const readyPlayer = async () => {
    await readyUp(gameCode);
  };

  const unreadyPlayer = async () => {
    await unready(gameCode);
  };

  const copyCodeToClipboard = () => {
    navigator.clipboard.writeText(gameCode);
    createAlert("Copied party code.");
  };

  const isMe = (playerId: string) => {
    return playerId === currPlayer?.id;
  };

  return (
    <div className="flex flex-col gap-3 flex-1 max-w-[300px] bg-slate-800 bg-opacity-90 h-fit border border-orange-300 border-l-0">
      <header className="border-b border-orange-300 p-4">
        <h2 className="text-2xl text-orange-300">Lobby</h2>
      </header>
      <section className="p-3 text-slate-100">
        <ul className="flex flex-col gap-3">
          <li
            className={`flex items-center gap-2 p-3 bg-slate-700 rounded-full ${
              isMe(players[0].id) ? "text-orange-200" : ""
            }`}
          >
            {players[0].isReady ? (
              <MdCheck className="text-green-500" />
            ) : (
              <Waiting />
            )}
            {players[0].name} {isPlayerReady(players[0].id)}
          </li>
          {players.length === 2 ? (
            <li
              className={`flex items-center gap-2 p-3 bg-slate-700 rounded-full ${
                isMe(players[1].id) ? "text-orange-200" : ""
              }`}
            >
              {players[1].isReady ? (
                <MdCheck className="text-green-500" />
              ) : (
                <Waiting />
              )}
              {players[1].name} {isPlayerReady(players[1].id)}
            </li>
          ) : (
            <li
              className={`flex items-center gap-2 p-3 bg-slate-700 bg-opacity-50 rounded-full`}
            >
              Waiting for player...
            </li>
          )}
        </ul>
        <div className="flex gap-2 items-center text-white mt-3">
          <span className="font-bold">PARTY CODE:</span>
          <button
            className="p-2 border border-orange-300 rounded-full flex items-center gap-2 px-4 hover:bg-slate-700 transition-colors duration-200 active:translate-y-[1px]"
            onClick={copyCodeToClipboard}
          >
            {gameCode}
            <BiClipboard />
          </button>
        </div>
        <Chat />
      </section>

      <div className="flex gap-1">
        <button
          className={`p-6 border-orange-300 border-t transition-colors duration-500 ${
            currPlayer?.isReady
              ? "bg-slate-400 hover:bg"
              : "text-orange-300 hover:bg-orange-300 hover:text-slate-900"
          } min-w-36 flex-1`}
          onClick={async () => {
            if (!currPlayer?.isReady) {
              await readyPlayer();
            } else {
              await unreadyPlayer();
            }
          }}
        >
          {currPlayer?.isReady ? "Unready" : "Ready"}
        </button>
      </div>
    </div>
  );
};

export default PlayersSection;
