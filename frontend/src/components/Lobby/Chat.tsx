import { useState } from "react";
import { IoMdSend } from "react-icons/io";
import { useSignalR } from "../../SignalR/SignalRProvider";

const Chat = () => {
  const [message, setMessage] = useState('');
  const { messageLog, sendMessage } = useSignalR();

  return (
    <div>
      <ul>
        {messageLog.map((message, i) => <li key={`m-${i}`}>{message}</li>)}
      </ul>
      <div className="flex items-center">
        <span>Message:</span>
        <input
          className="border border-slate-500"
          type="text"
          value={message}
          onChange={(e) => setMessage(e.target.value)} />
        <button
          aria-label="Send button"
          className="bg-orange-300 flex items-center gap-2 px-2 py-1"
          onClick={() => {
            sendMessage(message);
            setMessage('');
          }}>
          Send
          <IoMdSend />
        </button>
      </div>
    </div>
  )
}

export default Chat