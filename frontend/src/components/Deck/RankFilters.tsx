import { useFilters } from "./FilterProvider";

const RankFilters = () => {
  const { setFilters, filters } = useFilters();

  return (
    <div>
      <div>
        Rank
      </div>
      <div className="flex">
        {[1, 2, 3, 0].map(rank => (
          <div key={`rank-${rank}`}>
            <label>{!rank ? "Replace" : rank}</label>
            <input
              type="checkbox"
              checked={filters.rank.includes(rank)}
              onChange={(e) => {
                if (e.target.checked) {
                  setFilters(filters => ({ ...filters, rank: [...filters.rank, rank] }))
                } else {
                  setFilters(filters => ({ ...filters, rank: filters.rank.filter(r => r != rank) }))
                }
              }} />
          </div>
        ))}
      </div>
    </div>
  )
}

export default RankFilters