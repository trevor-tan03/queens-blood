import { createContext, useContext, useRef } from "react";
import "../../../public/css/Alert.css";

interface IAlertContext {
  createAlert: (message: string) => void;
}

const AlertContext = createContext<IAlertContext>({
  createAlert: () => {},
});

export const AlertProvider = ({ children }: { children: React.ReactNode }) => {
  const alertContainerRef = useRef<HTMLDivElement>(null);
  const alertCount = useRef(0);

  const createAlert = (message: string) => {
    if (alertContainerRef.current) {
      const alertId = `alert-${alertCount.current}`;

      alertContainerRef.current.innerHTML += `<div id="${alertId}" class="alert text-orange-300 bg-slate-900 p-3 fade-">${message}</div>`;
      alertCount.current++;

      setTimeout(() => {
        document.querySelector(`#${alertId}`)?.remove();
      }, 5000);
    }
  };

  return (
    <AlertContext.Provider value={{ createAlert }}>
      <div className="w-dvw h-dvh relative">
        {children}

        <div className="absolute bottom-3 right-3">
          <div
            id="alert-container"
            className="relative flex flex-col-reverse gap-1 *:rounded-md *:border-orange-300 *:border"
            ref={alertContainerRef}
          >
            {/* Alerts are added in here */}
          </div>
        </div>
      </div>
    </AlertContext.Provider>
  );
};

export const useAlertContext = () => {
  const context = useContext(AlertContext);
  if (!context) {
    throw new Error("useAlertContext must be used within a AlertContext");
  }
  return context;
};
