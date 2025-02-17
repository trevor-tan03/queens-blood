import { useCallback, useEffect, useState } from "react";
import { useSignalR } from "../../SignalR/SignalRProvider";
import { getTimeFormat } from "../../utils/formatDate";
import MulliganCardsScreen from "../Mulligan/MulliganCardsScreen";
import { default as CardsInHand } from "./Hand";

const Board = () => {
  const [cardsToMulligan, setCardsToMulligan] = useState<number[]>([]);
  const { hand, gameCode, mulliganPhaseEnded, getHand, mulliganCards } =
    useSignalR();
  const [hasMulliganed, setHasMulliganed] = useState(false);
  const [deadline] = useState<Date>(new Date(new Date().getTime() + 30500));

  useEffect(() => {
    getHand(gameCode);
  }, [gameCode, getHand]);

  const mulliganCardAtIndex = (cardIndex: number) => {
    if (cardsToMulligan.includes(cardIndex)) {
      setCardsToMulligan((c) => c.filter((index) => index !== cardIndex));
    } else {
      setCardsToMulligan((c) => [...c, cardIndex]);
    }
  };

  const confirmMulligan = useCallback(async () => {
    await mulliganCards(gameCode, cardsToMulligan);
    setCardsToMulligan([]);
    setHasMulliganed(true);
  }, [cardsToMulligan]);

  useEffect(() => {
    if (getTimeFormat(deadline) === 0) {
      confirmMulligan();
    }

    return;
  }, [getTimeFormat(deadline), deadline]);

  const isSelectedToMulligan = (index: number) =>
    cardsToMulligan.includes(index);

  if (!mulliganPhaseEnded) {
    return (
      <div className="relative">
        <MulliganCardsScreen
          cards={hand}
          mulliganCardAtIndex={mulliganCardAtIndex}
          isSelectedToMulligan={isSelectedToMulligan}
          hasConfirmedMulligan={hasMulliganed}
          confirmMulligan={confirmMulligan}
          deadline={deadline}
        />
      </div>
    );
  }

  return (
    <div>
      <CardsInHand hand={hand} cardsToMulligan={cardsToMulligan} />
    </div>
  );
};

export default Board;
