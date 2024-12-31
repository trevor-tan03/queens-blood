
import { useOverlay } from "../Overlay";
import AbilityFilters from "./AbilityFilters";
import { useFilters } from "./FilterProvider";
import RankFilters from "./RankFilters";
import RarityFilters from "./RarityFilters";

const FilterModal = () => {
  const { hideOverlay } = useOverlay();
  const { applyFilters } = useFilters();

  return (
    <div
      className="absolute h-dvh w-full grid place-items-center z-50"
    >
      <div
        className="bg-slate-800 w-[60%] flex flex-col gap-2 p-8 z-[99] top-0 border border-orange-300"
      >
        <header className="flex justify-between items-center border-b border-orange-300 py-2
        ">
          <h2 className="text-3xl text-orange-300">Filters</h2>
          <div className="flex gap-3">
            <button
              type="reset"
              className="py-2 px-5 rounded-full border border-orange-300 text-orange-300"
              onClick={hideOverlay}>
              Close
            </button>
            <button
              type="button"
              onClick={() => {
                applyFilters();
                hideOverlay();
              }}
              className="py-2 px-5 rounded-full border border-orange-300 bg-orange-300 text-slate-800"
            >
              Apply filters
            </button>
          </div>
        </header>

        <RankFilters />
        <RarityFilters />
        <AbilityFilters />
      </div>

      <div className="bg-black bg-opacity-45 absolute h-full w-full" onClick={hideOverlay}></div>
    </div>
  )
}

export default FilterModal