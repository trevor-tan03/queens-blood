interface Props {
  rank: number;
  isMine: boolean;
}

const Pawn = ({ rank, isMine }: Props) => {
  const filename = isMine ? `green-${rank}` : `red-${rank}`;

  return (
    <div>
      <img
        alt={`${isMine ? "green" : "red"}-rank-${rank}`}
        src={`/assets/effects/${filename}.webp`}
      />
    </div>
  );
};

export default Pawn;
