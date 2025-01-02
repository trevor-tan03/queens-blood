
import { BiClipboard, BiExit } from "react-icons/bi";
import { MdCheck, MdHourglassBottom } from "react-icons/md";
import { useSignalR } from "../../SignalR/SignalRProvider";
import Chat from "./Chat";

const PlayersSection = () => {
  const { gameCode, players, leaveGame, readyUp, unready, currPlayer } = useSignalR();

  const isPlayerReady = (connectionId: string) => {
    return players.find(player => player.id === connectionId)?.isReady;
  }

  const disconnect = async () => {
    await leaveGame(gameCode);
    window.location.replace("/")
  }

  const readyPlayer = async () => {
    await readyUp(gameCode);
  }

  const unreadyPlayer = async () => {
    await unready(gameCode);
  }

  const copyCodeToClipboard = () => {
    navigator.clipboard.writeText(gameCode);
  }

  return (
    <div className="flex flex-col gap-3 flex-1">
      <section className="border border-orange-300 p-3 bg-slate-700 text-slate-50">
        <ul className="flex flex-col gap-3">
          <li
            className={`flex items-center gap-2 p-3 bg-orange-300 rounded-full`}>
            {
              players[0].isReady
                ?
                <MdCheck className="text-green-700" />
                :
                <MdHourglassBottom className="text-gray-500" />
            }
            {players[0].name} {isPlayerReady(players[0].id)}
          </li>
          {players.length === 2 ? <li
            className={`flex items-center gap-2 p-3 bg-orange-300 rounded-full`}>
            {
              players[1].isReady
                ?
                <MdCheck className="text-green-700" />
                :
                <MdHourglassBottom className="text-gray-500" />
            }
            {players[1].name} {isPlayerReady(players[1].id)}
          </li> : <li
            className={`flex items-center gap-2 p-3 bg-orange-300 bg-opacity-50 rounded-full`}>
            Waiting for player...
          </li>}
        </ul>
        <Chat />
      </section>

      <div className="flex gap-2 items-center text-white">
        <span className="font-bold">
          PARTY CODE:
        </span>
        <button
          className="p-2 border border-orange-300 rounded-full flex items-center gap-2 px-4 hover:bg-slate-700 transition-colors duration-200"
          onClick={copyCodeToClipboard}>
          {gameCode}
          <BiClipboard />
        </button>
      </div>

      <button
        className="p-2 rounded-full bg-orange-300 min-w-36"
        onClick={async () => {
          if (!currPlayer?.isReady) {
            await readyPlayer();
          } else {
            await unreadyPlayer();
          }
        }}>
        {currPlayer?.isReady ? "Unready" : "Ready"}
      </button>

      <button
        className="flex gap-2 items-center mt-auto ml-auto py-3 px-8 rounded-full text-orange-300 hover:bg-red-700 hover:text-slate-50 transition-colors duration-200"
        onClick={disconnect}>
        <BiExit />
        Leave
      </button>
    </div>
  )
}

export default PlayersSection