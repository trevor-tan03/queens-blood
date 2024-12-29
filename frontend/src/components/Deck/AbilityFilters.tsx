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

  const handleOnChange = (e: React.ChangeEvent<HTMLInputElement>, ability: Ability) => {
    console.log(ability);
    if (e.target.checked) {
      setFilters(filters => ({ ...filters, ability: [...filters.ability, ability] }))
    } else {
      setFilters(filters => ({ ...filters, ability: filters.ability.filter(a => a != ability) }))
    }
  }

  return (
    <div>
      <div>
        Ability
      </div>
      <div className="flex">
        {Object.keys(abilityMap).filter(k => isNaN(Number(k))).map(ability => (
          <div key={`ability-${ability}`}>
            <label>{abilityMap[ability as keyof typeof abilityMap]}</label>
            <input
              type="checkbox"
              checked={filters.ability.includes(ability as Ability)}
              onChange={(e) => handleOnChange(e, ability as Ability)}
            />
          </div>
        ))}
      </div>
    </div>
  )
}

export default AbilityFilters