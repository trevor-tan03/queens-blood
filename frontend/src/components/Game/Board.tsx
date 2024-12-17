import { useEffect, useState } from "react";
import { useSignalR } from "../../SignalR/SignalRProvider";

const Board = () => {
  const [cardsToMulligan, setCardsToMulligan] = useState<number[]>([]);
  const { hand, gameCode, mulliganPhaseEnded, getHand, mulliganCards } = useSignalR();

  useEffect(() => {
    console.log(mulliganPhaseEnded)
  }, [mulliganPhaseEnded])

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

  const confirmMulligan = async () => {
    await mulliganCards(gameCode, cardsToMulligan);
    setCardsToMulligan([]);
  }

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
        <button onClick={confirmMulligan}>Confirm</button>
      </div>
    )
  }

  return (
    <div>Game Time</div>
  )
}

export default Board