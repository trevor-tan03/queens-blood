import { createContext, useContext, useState } from "react";

interface IOverlayContext {
  show: boolean;
  showOverlay: () => void;
  hideOverlay: () => void;
}

const OverlayContext = createContext<IOverlayContext | undefined>(undefined);

export const OverlayProvider = ({ children }: { children: React.ReactNode }) => {
  const [show, setShow] = useState(false);

  const showOverlay = () => setShow(true);
  const hideOverlay = () => setShow(false);

  return (
    <OverlayContext.Provider value={{ show, showOverlay, hideOverlay }}>
      {children}
    </OverlayContext.Provider>
  );
}

export const useOverlay = (): IOverlayContext => {
  const context = useContext(OverlayContext);

  if (!context) {
    throw new Error("useOverlay must be used within a OverlayProvider");
  }

  return context;
};
