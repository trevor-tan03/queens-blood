import { useEffect, useState } from "react";
import { useNavigate } from "react-router";
import { useSignalR } from "../../SignalR/SignalRProvider";

const HomeMenu = () => {
  const [name, setName] = useState('');
  const [code, setCode] = useState('');
  const { gameCode, createGame, joinGame } = useSignalR();
  const navigate = useNavigate();

  useEffect(() => {
    if (gameCode) {
      navigate(`/game/${gameCode}`);
    }
  }, [gameCode, navigate]);


  return (
    <div className="flex flex-col span px-24 bg-slate-700 items-center *:w-full gap-3 py-32 border border-orange-300">
      <h1 className="text-4xl font-bold text-center text-orange-300 mb-8">
        {"Queen's Blood"}
      </h1>
      <input
        id="name"
        className="border border-slate-500 p-2 px-6 rounded-full mb-4"
        placeholder="Enter your name"
        type="text"
        value={name}
        onChange={((e) => setName(e.target.value))}
      />
      {/* <label htmlFor="code">Code</label>
        <input id="code" className="border border-slate-500" type="text" value={code} onChange={((e) => setCode(e.target.value))} /> */}
      <button
        className="text-slate-700 bg-orange-300 p-2 rounded-full"
        onClick={async () => await createGame(name)}
      >
        Create
      </button>
      <button
        className="text-slate-700 bg-orange-300 p-2 rounded-full"
        onClick={async () => await joinGame(code, name)}
      >
        Join
      </button>
      <button
        className="text-orange-300 border border-orange-300 p-2 rounded-full"
        onClick={() => navigate("/deck")}
      >
        Change Deck
      </button>
    </div>
  )
}

export default HomeMenu