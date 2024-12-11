import { useSignalR } from "../SignalR/SignalRProvider";

const Game = () => {
  const { gameCode, players, leaveGame } = useSignalR();

  const disconnect = async () => {
    await leaveGame(gameCode);
    window.location.replace("/")
  }

  return (
    <>
      <div>{gameCode}</div>
      <ul>
        {players.map(player => <li key={player.id}>{player.name}</li>)}
      </ul>
      <button onClick={disconnect}>Leave</button>
    </>
  )
}

export default Game