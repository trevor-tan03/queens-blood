import { useFilters } from "./FilterProvider";

const RarityFilters = () => {
  const { setFilters, filters } = useFilters();

  const handleClick = (rarity: "Standard" | "Legendary") => {
    if (!filters.rarity.includes(rarity)) {
      setFilters(filters => ({ ...filters, rarity: [...filters.rarity, rarity] }));
    } else {
      setFilters(filters => ({ ...filters, rarity: filters.rarity.filter(r => r != rarity) }));
    }
  }

  return (
    <div>
      <div className="text-3xl text-orange-300 mt-3">
        Rarity
      </div>
      <div className="flex gap-3">
        {(["Standard", "Legendary"] as ("Standard" | "Legendary")[]).map((rarity) => (
          <button
            key={`rarity-${rarity}`}
            className={`py-2 px-4 w-40 rounded-full border border-orange-300 ${filters.rarity.includes(rarity) ? "bg-orange-300 text-slate-900" : "bg-slate-600 text-orange-300"}`}
            onClick={() => handleClick(rarity as "Standard" | "Legendary")}
          >
            {rarity}
          </button>
        ))}
      </div>
    </div>
  )
}

export default RarityFilters