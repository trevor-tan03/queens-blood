import { useEffect, useRef, useState } from "react";
import { useSignalR } from "../../SignalR/SignalRProvider";

const Chat = () => {
  const [message, setMessage] = useState("");
  const { messageLog, sendMessage, currPlayer } = useSignalR();
  const chatRef = useRef<HTMLUListElement | null>(null);

  useEffect(() => {
    chatRef.current?.scrollTo(0, chatRef.current.scrollHeight);
  }, [messageLog]);

  return (
    <div>
      <ul
        className="max-h-40 overflow-y-scroll mt-6 border-t border-gray-500 text-white"
        ref={chatRef}
      >
        {messageLog.map((msgData, i) => (
          <li
            className={`${
              msgData.playerId === currPlayer?.id
                ? "text-right bg-green-400 text-slate-700 ml-auto"
                : msgData.playerId === "Server"
                ? "text-center text-gray-500"
                : "text-left bg-red-400"
            } rounded-lg w-fit p-1 mb-1 text-slate-700`}
            key={`m-${i}`}
          >
            <span>{msgData.message}</span>
          </li>
        ))}
      </ul>
      <div className="flex items-center bg-slate-900">
        <input
          placeholder="Enter message"
          className="border outline-none bg-transparent border-transparent focus:border-orange-300 w-full p-2 h-full text-white"
          type="text"
          value={message}
          onChange={(e) => setMessage(e.target.value)}
          onKeyDown={(e) => {
            if (e.key === "Enter") {
              sendMessage(message);
              setMessage("");
            }
          }}
        />
      </div>
    </div>
  );
};

export default Chat;
