import { useEffect, useState } from "react";
import { useNavigate } from "react-router";
import { useSignalR } from "../SignalR/SignalRProvider";

const Home = () => {
  const [name, setName] = useState('');
  const [code, setCode] = useState('');
  const { gameCode, createGame, joinGame, leaveGame } = useSignalR();

  const navigate = useNavigate();

  useEffect(() => {
    if (gameCode) {
      console.log(gameCode);
      navigate(`/game/${gameCode}`);
    }
  }, [gameCode, navigate]);

  return (
    <div>
      <div>
        <label htmlFor="name">Name</label>
        <input id="name" className="border border-slate-500" type="text" value={name} onChange={((e) => setName(e.target.value))} />
        <button className="bg-blue-300" onClick={async () => await createGame(name)}>
          Create
        </button>
      </div>
      <div>
        <label htmlFor="code">Code</label>
        <input id="code" className="border border-slate-500" type="text" value={code} onChange={((e) => setCode(e.target.value))} />
        <button onClick={async () => await joinGame(code, name)}>
          Join
        </button>
      </div>
      <div>
        <button onClick={() => navigate("/deck")}>
          Deck
        </button>
      </div>
    </div>
  )
}

export default Home