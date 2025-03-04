import { createContext, useContext, useState } from "react";
import { useSignalR } from "../../../SignalR/SignalRProvider";

interface ICompatibleCardsContext {
  compatibleTiles: string[];
  getCompatibleTiles: (selectedCardRank: number) => void;
  resetCompatibleTiles: () => void;
}

const CompatibleCardsContext = createContext<ICompatibleCardsContext>({
  compatibleTiles: [],
  getCompatibleTiles: () => {},
  resetCompatibleTiles: () => {},
});

export const CompatibleCardsProvider = ({
  children,
}: {
  children: React.ReactNode;
}) => {
  const [compatibleTiles, setCompatibleTiles] = useState<string[]>([]);
  const { gameState, playing, currPlayer } = useSignalR();

  const getCompatibleTiles = (selectedCardRank: number) => {
    if (!gameState) return;
    const compatibleTiles: string[] = [];

    for (let i = 0; i < 3; i++) {
      for (let j = 0; j < 5; j++) {
        const tileOwner = gameState.board[i][j].ownerId;
        const eligibleRank = gameState.board[i][j].rank >= selectedCardRank;
        const currentlyPlaying = playing === currPlayer?.id;
        const tileOccupied = gameState.board[i][j].card;

        if (
          eligibleRank &&
          currentlyPlaying &&
          !tileOccupied &&
          currPlayer.id === tileOwner
        )
          compatibleTiles.push(`${i},${j}`);
      }
    }

    setCompatibleTiles(compatibleTiles);
  };

  const resetCompatibleTiles = () => {
    setCompatibleTiles([]);
  };

  return (
    <CompatibleCardsContext.Provider
      value={{ compatibleTiles, getCompatibleTiles, resetCompatibleTiles }}
    >
      {children}
    </CompatibleCardsContext.Provider>
  );
};

export const useCompatibleTiles = () => {
  const context = useContext(CompatibleCardsContext);
  if (!context) {
    throw new Error("useCardAbility must be used within a CardAbilityContext");
  }
  return context;
};
