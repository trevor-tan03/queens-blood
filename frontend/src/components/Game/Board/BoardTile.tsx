import { useBoardContext } from "./BoardContext";

const BoardTile = () => {
  const { child } = useBoardContext();

  return <div className="w-[170px] h-[200px]">{child}</div>;
};

export default BoardTile;
