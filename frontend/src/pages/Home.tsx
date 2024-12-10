import { useState } from "react";
import { useSignalR } from "../SignalR/SignalRProvider";

const Home = () => {
  const [name, setName] = useState('');
  const [code, setCode] = useState('');
  const { createGame, joinGame, leaveGame } = useSignalR();

  return (
    <div>
      <div>
        <label htmlFor="name">Name</label>
        <input id="name" className="border border-slate-500" type="text" value={name} onChange={((e) => setName(e.target.value))} />
        <button className="bg-blue-300" onClick={() => createGame(name)}>
          Create
        </button>
      </div>
      <div>
        <label htmlFor="code">Code</label>
        <input id="code" className="border border-slate-500" type="text" value={code} onChange={((e) => setCode(e.target.value))} />
        <button onClick={() => joinGame(code, name)}>
          Join
        </button>
      </div>
      <div>
        <button onClick={() => leaveGame(code)}>
          Leave
        </button>
      </div>
    </div>
  )
}

export default Home