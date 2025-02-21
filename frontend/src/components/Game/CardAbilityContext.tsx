import { createContext, useContext, useState } from "react";

interface ICardAbilityContext {
  shownAbility: string;
  setShownAbility: React.Dispatch<React.SetStateAction<string>>;
}

const CardAbilityContext = createContext<ICardAbilityContext>({
  shownAbility: "",
  setShownAbility: () => {},
});

export const CardAbilityProvider = ({
  children,
}: {
  children: React.ReactNode;
}) => {
  const [shownAbility, setShownAbility] = useState("");

  return (
    <CardAbilityContext.Provider value={{ shownAbility, setShownAbility }}>
      {children}
    </CardAbilityContext.Provider>
  );
};

export const useCardAbility = () => {
  const context = useContext(CardAbilityContext);
  if (!context) {
    throw new Error("useCardAbility must be used within a CardAbilityContext");
  }
  return context;
};
