import React from "react";
import ChatBotIcon from "./ChatBotIcon";

const ChatMessage = ({ chat }) => {
  return (
    <div
      className={`flex gap-3 items-end message ${chat.role === "model" ? "justify-start" : "justify-end"} ${chat.isError ? "error" : ""}`}
    >
     
      {chat.role === "model" && (
        <div className="shrink-0 mb-1">
          <ChatBotIcon />
        </div>
      )}

      <p
        className={`py-3 px-4 max-w-[75%] text-sm
    ${
      chat.role === "model"
        ? "bg-lime-100 text-gray-800 rounded-[13px_13px_13px_3px]"
        : "bg-lime-600 text-white rounded-[13px_13px_3px_13px]"
    }`}
      >
        {chat.text}
      </p>
    </div>
  );
};

export default ChatMessage;
