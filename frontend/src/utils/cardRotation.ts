const leftCardRotation = -3;

export const getXOffset = (index: number) => index * 160;

export const getYOffset = (index: number, numCardsInHand: number) => {
  return -Math.sin((index * Math.PI) / numCardsInHand) * 10;
};

export const getRotation = (index: number, numCardsInHand: number) => {
  return (
    leftCardRotation + (index / (numCardsInHand - 1)) * (3 - leftCardRotation)
  );
};
