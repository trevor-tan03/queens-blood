import { useEffect, useRef, useState } from "react";
import { useSignalR } from "../../SignalR/SignalRProvider";

const Chat = () => {
  const [message, setMessage] = useState('');
  const { messageLog, sendMessage } = useSignalR();
  const chatRef = useRef<HTMLUListElement | null>(null);

  useEffect(() => {
    chatRef.current?.scrollTo(0, chatRef.current.scrollHeight);
  }, [messageLog])

  return (
    <div>
      <ul className="max-h-40 overflow-y-scroll mt-6 border-t border-gray-500" ref={chatRef}>
        {messageLog.map((message, i) => <li key={`m-${i}`}>{message}</li>)}
      </ul>
      <div className="flex items-center bg-slate-900">
        <input
          placeholder="Enter message"
          className="border outline-none bg-transparent border-transparent focus:border-orange-300 w-full p-2 h-full"
          type="text"
          value={message}
          onChange={(e) => setMessage(e.target.value)}
          onKeyDown={(e) => {
            if (e.key === "Enter") {
              sendMessage(message);
              setMessage('');
            }
          }}
        />
      </div>
    </div>
  )
}

export default Chat