import { useState } from "react";
import { useNavigate } from "react-router";
import { useSignalR } from "../SignalR/SignalRProvider";
import Board from "../components/Game/Board";

const Game = () => {
  const { messageLog, connection, gameCode, players, leaveGame, readyUp, unready, sendMessage, currPlayer, gameStart } = useSignalR();
  const [message, setMessage] = useState('');
  const navigate = useNavigate();

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

  const readyPlayer = async () => {
    await readyUp(gameCode);
  }

  const unreadyPlayer = async () => {
    await unready(gameCode);
  }

  return (
    <>
      {gameStart ? <Board /> :
        <>
          <div>{gameCode}</div>
          <button onClick={async () => {
            if (!currPlayer?.isReady) {
              await readyPlayer();
            } else {
              await unreadyPlayer();
            }
          }}>
            {currPlayer?.isReady ? "Unready" : "Ready"}
          </button>

          <ul>
            {players.map(player => <li key={player.id} className={player.id === connectionId ? "text-red-700" : ""}>
              {isPlayerHost(player.id) && "👑"} {player.name} {isPlayerReady(player.id) && "(READY)"}
            </li>)}
          </ul>
          <button onClick={disconnect}>Leave</button>

          <button onClick={() => {
            navigate(`/game/${gameCode}/deck`)
          }}>Decks</button>

          <input type="text" value={message} onChange={(e) => setMessage(e.target.value)} />
          <button onClick={() => {
            sendMessage(message);
            setMessage('');
          }}>Send</button>

          <ul>
            {messageLog.map((message, i) => <li key={`m-${i}`}>{message}</li>)}
          </ul>
        </>}

    </>
  )
}

export default Game