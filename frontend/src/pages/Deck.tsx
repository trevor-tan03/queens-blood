import { useQuery } from "@tanstack/react-query";
import { useNavigate } from "react-router";
import { useSignalR } from "../SignalR/SignalRProvider";
import { Card } from "../types/Card";

const Deck = () => {
  const { gameCode } = useSignalR();
  const navigate = useNavigate();

  const api = `${import.meta.env.VITE_API_URL}/api/Card`;

  const { isPending, error, data } = useQuery<Card[]>({
    queryKey: ["baseCards"],
    queryFn: () => fetch(api).then((res) => res.json()),
    staleTime: 60000,
  });

  if (isPending) return "Loading...";

  if (error) return "An error occurred: " + error.message;

  return (
    <div className="">
      <button onClick={() => {
        navigate(`/game/${gameCode}`);
      }}>Save</button>

      <div className="grid grid-cols-11 max-w-full overflow-y-scroll gap-2">
        {data.map(d => <div>
          <img src={`../../public/assets/cards/${d.image}`} alt={d.name} loading="lazy" />
          <div>{d.rarity === "Standard" ? "2/2" : "1/1"}</div>
        </div>)}
      </div>
    </div>
  )
}

export default Deck