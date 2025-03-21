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
        className="h-[20.5rem] overflow-y-auto mt-6 border-t border-gray-500 text-white"
        ref={chatRef}
      >
        {messageLog.map((msgData, i) => (
          <li
            className={`${
              msgData.playerId === currPlayer?.id
                ? "text-white ml-auto bg-[#785a28]"
                : msgData.playerId === "Server"
                ? "text-center text-gray-400 mx-auto text-sm"
                : "bg-[#1e2328] text-white"
            } w-fit mb-1 text-slate-700 px-3 py-1 rounded-md`}
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
