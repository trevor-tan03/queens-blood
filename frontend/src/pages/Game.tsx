import { useState } from "react";
import { useSignalR } from "../SignalR/SignalRProvider";

const Game = () => {
  const { messageLog, connection, gameCode, players, leaveGame, readyUp, sendMessage, currPlayer } = useSignalR();
  const [message, setMessage] = useState('');

  const isPlayerReady = (connectionId: string) => {
    return players.find(player => player.id === connectionId)?.isReady;
  }

  const isPlayerHost = (connectionId: string) => {
    return players.find(player => player.id === connectionId)?.isHost;
  }

  const connectionId = connection!.connectionId;

  const disconnect = async () => {
    await leaveGame(gameCode);
    window.location.replace("/")
  }

  const toggleReady = async () => {
    await readyUp(gameCode);
  }

  const isLobbyReady = () => {
    return players.length === 2 && players[0].isReady && players[1].isReady;
  }

  return (
    <>
      <div>{gameCode}</div>
      {currPlayer?.isHost ?
        <button className="disabled:bg-slate-300 bg-green-300" disabled={!isLobbyReady()}>
          Start
        </button>
        :
        <button onClick={toggleReady}>
          {currPlayer?.isReady ? "Unready" : "Ready"}
        </button>
      }
      <ul>
        {players.map(player => <li key={player.id} className={player.id === connectionId ? "text-red-700" : ""}>
          {isPlayerHost(player.id) && "👑"} {player.name} {isPlayerReady(player.id) && "(READY)"}
        </li>)}
      </ul>
      <button onClick={disconnect}>Leave</button>

      <input type="text" value={message} onChange={(e) => setMessage(e.target.value)} />
      <button onClick={() => {
        sendMessage(message);
        setMessage('');
      }}>Send</button>

      <ul>
        {messageLog.map((message, i) => <li key={`m-${i}`}>{message}</li>)}
      </ul>
    </>
  )
}

export default Game