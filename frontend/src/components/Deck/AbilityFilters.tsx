import { Ability } from "../../types/Card";
import { useFilters } from "./FilterProvider";

const AbilityFilters = () => {
  const { filters, setFilters } = useFilters();

  const abilityMap = {
    [Ability.AddToHand]: "Add Cards to Hand",
    [Ability.Destroy]: "Destroy Cards",
    [Ability.Enfeeble]: "Enfeeble Cards",
    [Ability.Enhance]: "Enhance Cards",
    [Ability.IncreaseRank]: "Raise Position Rank",
    [Ability.Spawn]: "Spawn Cards",
    [Ability.ScoreBonus]: "Adds Score Bonus",
  }

  const handleClick = (ability: Ability) => {
    if (!filters.ability.includes(ability)) {
      setFilters(filters => ({ ...filters, ability: [...filters.ability, ability] }))
    } else {
      setFilters(filters => ({ ...filters, ability: filters.ability.filter(a => a != ability) }))
    }
  }

  return (
    <div>
      <div className="text-3xl text-orange-300 mt-3">
        Ability
      </div>
      <div className="grid grid-cols-2 gap-3">
        {Object.keys(abilityMap).filter(k => isNaN(Number(k))).map(ability => (
          <button
            className={`py-2 px-4 w-full rounded-full border border-orange-300 ${filters.ability.includes(ability as Ability) ? "bg-orange-300 text-slate-900" : "bg-slate-600 text-orange-300"}`}
            key={`ability-${ability}`}
            onClick={() => handleClick(ability as Ability)}
          >
            {abilityMap[ability as keyof typeof abilityMap]}
          </button>
        ))}
      </div>
    </div>
  )
}

export default AbilityFilters