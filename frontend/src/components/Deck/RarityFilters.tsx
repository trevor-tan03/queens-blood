import { useFilters } from "./FilterProvider";

const RarityFilters = () => {
  const { setFilters, filters } = useFilters();
  return (
    <div>
      <div>
        Rarity
      </div>
      <div className="flex">
        {["Standard", "Legendary"].map((rarity) => (
          <div key={`rarity-${rarity}`}>
            <label>{rarity}</label>
            <input
              type="checkbox"
              checked={filters.rarity.includes(rarity as "Standard" | "Legendary")}
              onChange={(e) => {
                if (e.target.checked) {
                  setFilters(filters => ({ ...filters, rarity: [...filters.rarity, rarity as "Standard" | "Legendary"] }))
                } else {
                  setFilters(filters => ({ ...filters, rarity: filters.rarity.filter(r => r != rarity) }))
                }
              }} />
          </div>
        ))}
      </div>
    </div>
  )
}

export default RarityFilters