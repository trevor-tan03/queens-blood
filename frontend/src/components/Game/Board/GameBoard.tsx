import { useSignalR } from "../../../SignalR/SignalRProvider";
import { useCardAbility } from "../CardAbilityContext";
import CardsInHand from "../Hand";

const GameBoard = () => {
  const { hand } = useSignalR();
  const { shownAbility } = useCardAbility();

  return (
    <div>
      <div className="relative">
        <CardsInHand hand={hand} />
      </div>

      <div className="absolute bottom-[1rem] left-[1rem] text-2xl">
        {shownAbility}
      </div>
    </div>
  );
};

export default GameBoard;
