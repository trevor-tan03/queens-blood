import { useCallback, useEffect, useState } from "react";
import { useSignalR } from "../../SignalR/SignalRProvider";
import { getTimeFormat } from "../../utils/formatDate";

const Board = () => {
  const [cardsToMulligan, setCardsToMulligan] = useState<number[]>([]);
  const { hand, gameCode, mulliganPhaseEnded, getHand, mulliganCards } = useSignalR();
  const [hasMulliganed, setHasMulliganed] = useState(false);

  const [mulliganTimer, setMulliganTimer] = useState<ReturnType<typeof setTimeout> | null>(null);
  const [deadline] = useState<Date>(new Date(new Date().getTime() + 30500));

  useEffect(() => {
    getHand(gameCode);
  }, [gameCode, getHand]);

  const mulliganCard = (cardIndex: number) => {
    if (cardsToMulligan.includes(cardIndex)) {
      setCardsToMulligan(c => c.filter(index => index !== cardIndex));
    } else {
      setCardsToMulligan(c => [...c, cardIndex]);
    }
  }

  const confirmMulligan = useCallback(async () => {
    await mulliganCards(gameCode, cardsToMulligan);
    setCardsToMulligan([]);
    setHasMulliganed(true);
  }, [cardsToMulligan])

  useEffect(() => {
    if (getTimeFormat(deadline) === 0) {
      confirmMulligan();
    }

    return;
  }, [getTimeFormat(deadline), deadline])

  if (!mulliganPhaseEnded) {
    return (
      <div>
        Mulligan
        <div className="grid grid-cols-5">
          {hand.map((card, i) => (
            <div>
              <img
                draggable={false}
                src={`../../../../assets/cards/${card.image}`}
                alt={card.name}
                key={`hand-${i}`}
                onClick={() => mulliganCard(i)}
              />
              {cardsToMulligan.includes(i) ? <span>
                Mulligan
              </span> : null}
            </div>
          ))}
        </div>
        {!hasMulliganed && <button onClick={confirmMulligan}>Confirm</button>}
        <div>{getTimeFormat(deadline)}</div>
      </div>
    )
  }

  return (
    <div className="grid grid-cols-5">
      {hand.map((card, i) => (
        <div>
          <img
            draggable={false}
            src={`../../../../assets/cards/${card.image}`}
            alt={card.name}
            key={`hand-${i}`}
          />
        </div>
      ))}
    </div>
  )
}

export default Board