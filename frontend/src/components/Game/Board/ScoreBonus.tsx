interface Props {
  isWinningLane: boolean;
  bonusPoints: number;
  isMine: boolean;
}

const ScoreBonus = ({ isWinningLane, bonusPoints, isMine }: Props) => {
  return (
    <span
      className={`
        p-2 px-4 border
        ${
          isWinningLane
            ? "text-yellow-200 border-yellow-200 font-bold"
            : "text-gray-300 border-gray-200"
        }
        ${
          isMine
            ? "right-0 rounded-l-full bg-blue-900"
            : "left-0 rounded-r-full bg-red-900"
        }
        absolute translate-y-16`}
    >
      +{bonusPoints}
    </span>
  );
};

export default ScoreBonus;
