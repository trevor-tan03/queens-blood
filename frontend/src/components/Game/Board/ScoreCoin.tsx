interface Props {
  value: number;
  isMine: boolean;
}

const ScoreCoin = ({ value, isMine }: Props) => {
  return (
    <div
      className={`text-xl font-bold grid place-items-center text-yellow-200 w-[80px] h-[80px] rounded-full border-4 border-yellow-200 ${
        isMine ? "bg-blue-900" : "bg-red-900 ml-auto"
      }`}
    >
      {value}
    </div>
  );
};

export default ScoreCoin;
