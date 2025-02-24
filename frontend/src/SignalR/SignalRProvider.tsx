import * as signalR from "@microsoft/signalr";
import React, {
  createContext,
  ReactNode,
  useContext,
  useEffect,
  useState,
} from "react";
import type { Card } from "../types/Card";
import type { Game, GameDTO } from "../types/Game";
import type { Player } from "../types/Player";

interface SignalRContextProps {
  connection: signalR.HubConnection | null;
  createGame: (playerName: string) => Promise<void>;
  joinGame: (gameId: string, playerName: string) => Promise<void>;
  leaveGame: (gameId: string) => Promise<void>;
  readyUp: (gameId: string) => Promise<void>;
  unready: (gameId: string) => Promise<void>;
  sendMessage: (message: string) => Promise<void>;
  getHand: (gameId: string) => Promise<void>;
  mulliganCards: (gameId: string, cardsToMulligan: number[]) => Promise<void>;
  playCard: (
    gameId: string,
    cardIndex: number,
    row: number,
    col: number
  ) => Promise<void>;
  currPlayer: Player | undefined;
  gameCode: string;
  players: Player[];
  messageLog: string[];
  gameStart: boolean;
  hand: Card[];
  mulliganPhaseEnded: boolean;
  playing: string;
  gameState: Game | null;
}

const SignalRContext = createContext<SignalRContextProps | undefined>(
  undefined
);

export const SignalRProvider: React.FC<{ children: ReactNode }> = ({
  children,
}) => {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(
    null
  );
  const [gameCode, setGameCode] = useState("");
  const [currPlayer, setCurrPlayer] = useState<Player | undefined>();
  const [players, setPlayers] = useState<Player[]>([]);
  const [messageLog, setMessageLog] = useState<string[]>([]);
  const [gameStart, setGameStart] = useState(false);
  const [hand, setHand] = useState<Card[]>([]);
  const [mulliganPhaseEnded, setMulliganPhaseEnded] = useState(false);
  const [playing, setPlaying] = useState(""); //
  const [gameState, setGameState] = useState<Game | null>(null);

  const setupSignalREvents = (conn: signalR.HubConnection) => {
    conn.on("ReceiveMessage", (message: string) => {
      setMessageLog((prevLog) => [...prevLog, message]);
    });

    conn.on("ErrorMessage", (message: string) => {
      console.log(message);
    });

    conn.on("GameCode", (gameCode: string) => {
      setGameCode(gameCode);
    });

    conn.on("ReceivePlayerList", (players: Player[]) => {
      const currPlayer = players.find((p) => p.id === conn.connectionId);
      setCurrPlayer(currPlayer);
      setPlayers(players);
    });

    conn.on("GameStart", (start: boolean) => {
      setGameStart(start);
    });

    conn.on("CardsInHand", (hand: Card[]) => {
      setHand(hand);
    });

    conn.on("MulliganPhaseEnded", (hasEnded: boolean) => {
      setMulliganPhaseEnded(hasEnded);
    });

    conn.on("Playing", (playing: string) => {
      setPlaying(playing);
      console.log(playing);
    });

    conn.on("GameState", (gameState: GameDTO) => {
      const board = [
        gameState.board.slice(0, 5),
        gameState.board.slice(5, 10),
        gameState.board.slice(10, 15),
      ];

      const game: Game = {
        laneScores: gameState.laneScores,
        board,
      };

      setGameState(game);
    });
  };

  useEffect(() => {
    const connect = async () => {
      const apiUrl = `${import.meta.env.VITE_API_URL}/gameHub`;
      const conn = new signalR.HubConnectionBuilder()
        .withUrl(apiUrl)
        .configureLogging(signalR.LogLevel.Information)
        .build();

      setupSignalREvents(conn);
      await conn.start();
      setConnection(conn);
    };

    connect();

    return () => {
      if (connection) {
        connection.off("ReceiveMessage");
        connection.off("ErrorMessage");
        connection.off("GameCode");
        connection.off("ReceivePlayerList");
        connection.off("GameStart");
        connection.off("CardsInHand");
        connection.stop();
      }
    };
  }, []);

  useEffect(() => {
    const handleBeforeUnload = () => connection?.invoke("LeaveGame", gameCode);
    window.addEventListener("beforeunload", handleBeforeUnload);

    return () => window.removeEventListener("beforeunload", handleBeforeUnload);
  }, [connection, gameCode]);

  const handleError = (err: Error, context: string) => {
    console.error(`[${context}] Error:`, err);
  };

  const sendMessage = async (message: string) => {
    if (connection) {
      await connection.invoke("SendMessage", gameCode, message);
    }
  };

  const createGame = async (playerName: string) => {
    if (connection) {
      await connection
        .invoke("CreateGame", playerName)
        .catch((error) => handleError(error, "CreateGame"));
    }
  };

  const joinGame = async (gameId: string, playerName: string) => {
    if (connection) {
      await connection
        .invoke("JoinGame", gameId, playerName)
        .catch((error) => handleError(error, "JoinGame"));
      setGameCode(gameId);
    }
  };

  const leaveGame = async (gameId: string) => {
    if (connection) {
      await connection
        .invoke("LeaveGame", gameId)
        .catch((error) => handleError(error, "LeaveGame"));
    }
  };

  const readyUp = async (gameId: string) => {
    if (connection) {
      const rawDeck = localStorage.getItem("deck");

      if (rawDeck) {
        const deck = JSON.parse(rawDeck) as Card[];
        const cardIds = deck.map((card) => card.id);
        await connection.invoke("ToggleReady", gameId, true, cardIds);
      }
    }
  };

  const unready = async (gameId: string) => {
    if (connection) {
      await connection.invoke("ToggleReady", gameId, false, []);
    }
  };

  const getHand = async (gameId: string) => {
    if (connection) {
      await connection.invoke("GetHand", gameId);
    }
  };

  const mulliganCards = async (gameId: string, cardsToMulligan: number[]) => {
    if (connection) {
      connection.invoke("MulliganCards", gameId, cardsToMulligan);
    }
  };

  const playCard = async (
    gameId: string,
    cardIndex: number,
    row: number,
    col: number
  ) => {
    if (connection) {
      connection.invoke("PlaceCard", gameId, cardIndex, row, col);
    }
  };

  return (
    <SignalRContext.Provider
      value={{
        connection,
        createGame,
        joinGame,
        leaveGame,
        readyUp,
        unready,
        sendMessage,
        getHand,
        mulliganCards,
        playCard,
        gameCode,
        currPlayer,
        players,
        messageLog,
        gameStart,
        hand,
        mulliganPhaseEnded,
        playing,
        gameState,
      }}
    >
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
