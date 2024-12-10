import * as signalR from '@microsoft/signalr';
import React, { createContext, ReactNode, useContext, useEffect, useState } from 'react';
import { Methods } from './SignalRMethods';

interface SignalRContextProps {
  connection: signalR.HubConnection | null;
  createGame: (playerName: string) => void;
  joinGame: (gameId: string, playerName: string) => void;
  leaveGame: (gameId: string) => void;
}

const SignalRContext = createContext<SignalRContextProps | undefined>(undefined);

export const SignalRProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);

  useEffect(() => {
    const connect = async () => {
      const apiUrl = `${import.meta.env.VITE_API_URL}/gameHub`;
      const conn = new signalR.HubConnectionBuilder()
        .withUrl(apiUrl)
        .configureLogging(signalR.LogLevel.Information)
        .build();

      conn.on("ReceiveMessage", (message) => {
        console.log(message);
      });

      conn.on("ErrorMessage", (message) => {
        console.log(message);
      });

      conn.on("GameCode", (gameCode) => {
        console.log(gameCode)
      });

      await conn.start();
      setConnection(conn);
    };

    connect();

    return () => {
      if (connection) connection.stop();
    };
  }, []);

  const createGame = async (playerName: string) => {
    if (connection) {
      await connection.invoke(Methods.CreateGame, playerName).catch((error) => {
        return console.error(error.toString());
      })
    }
  }

  const joinGame = async (gameId: string, playerName: string,) => {
    if (connection) {
      await connection.invoke(Methods.JoinGame, gameId, playerName).catch((error) => {
        return console.error(error.toString());
      })
    }
  }

  const leaveGame = async (gameId: string) => {
    if (connection) {
      await connection.invoke(Methods.LeaveGame, gameId).catch((error) => {
        return console.error(error.toString());
      })
    }
  }

  return (
    <SignalRContext.Provider value={{
      connection,
      createGame,
      joinGame,
      leaveGame
    }}>
      {children}
    </SignalRContext.Provider>
  );
};

export const useSignalR = (): SignalRContextProps => {
  const context = useContext(SignalRContext);
  if (!context) {
    throw new Error("useSignalR must be used within a SignalRProvider");
  }
  return context;
};
