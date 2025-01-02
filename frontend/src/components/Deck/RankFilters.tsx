import { useFilters } from "./FilterProvider";

const RankFilters = () => {
  const { setFilters, filters } = useFilters();

  const handleClick = (rank: 0 | 1 | 2 | 3) => {
    if (!filters.rank.includes(rank)) {
      setFilters(filters => ({ ...filters, rank: [...filters.rank, rank] }));
    } else {
      setFilters(filters => ({ ...filters, rank: filters.rank.filter(r => r != rank) }));
    }
  }

  return (
    <div>
      <div className="text-3xl text-orange-300 mt-3">
        Rank
      </div>
      <div className="flex gap-3">
        {[1, 2, 3, 0].map(rank => (
          <button
            key={`rank-${rank}`}
            className={`py-2 px-4 w-40 rounded-full border border-orange-300 ${filters.rank.includes(rank) ? "bg-orange-300 text-slate-900" : "bg-slate-600 text-orange-300"}`}
            onClick={() => handleClick(rank as 0 | 1 | 2 | 3)}
          >
            {!rank ? "Replace" : rank}
          </button>
        ))}
      </div>
    </div>
  )
}

export default RankFilters