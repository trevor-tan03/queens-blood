import { useSignalR } from "../SignalR/SignalRProvider";

const Game = () => {
  const { connection, gameCode, players, leaveGame, readyUp } = useSignalR();

  console.log(players);

  const isPlayerReady = (connectionId: string) => {
    return players.find(player => player.id === connectionId)?.isReady;
  }

  const isPlayerHost = (connectionId: string) => {
    return players.find(player => player.id === connectionId)?.isHost;
  }

  const connectionId = connection!.connectionId;

  const isReady = isPlayerReady(connectionId);
  const isHost = isPlayerHost(connectionId);

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
      {isHost ?
        <button className="disabled:bg-slate-300 bg-green-300" disabled={!isLobbyReady()}>
          Start
        </button>
        :
        <button onClick={toggleReady}>
          {isReady ? "Unready" : "Ready"}
        </button>
      }
      <ul>
        {players.map(player => <li key={player.id} className={player.id === connectionId ? "text-red-700" : ""}>
          {isPlayerHost(player.id) && "👑"} {player.name} {isPlayerReady(player.id) && "(READY)"}
        </li>)}
      </ul>
      <button onClick={disconnect}>Leave</button>
    </>
  )
}

export default Game