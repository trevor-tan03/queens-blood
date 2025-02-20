import { UniqueIdentifier } from "@dnd-kit/core";
import { createContext, useContext, useState } from "react";

interface IBoardContext {
  child: UniqueIdentifier | null;
  setChild: React.Dispatch<React.SetStateAction<UniqueIdentifier | null>>;
}

const Context = createContext<IBoardContext>({
  child: null,
  setChild: () => {},
});

const BoardContext = ({ children }: { children: React.ReactNode }) => {
  const [child, setChild] = useState<UniqueIdentifier | null>(null);
  return (
    <Context.Provider value={{ child, setChild }}>{children}</Context.Provider>
  );
};

export default BoardContext;

export const useBoardContext = () => {
  const context = useContext(Context);
  if (!context) throw new Error("Invalid context");
  return context;
};
