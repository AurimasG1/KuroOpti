import React from "react";
import ChatBotIcon from "./ChatBotIcon";
import ReactMarkdown from "react-markdown";
import { Link } from "react-router-dom";

const ChatMessage = ({ chat }) => {
  return (
    <div
      className={`flex gap-3 items-end message ${
        chat.role === "model" ? "justify-start" : "justify-end"
      } ${chat.isError ? "error" : ""}`}
    >
      {chat.role === "model" && (
        <div className="shrink-0 mb-1">
          <ChatBotIcon />
        </div>
      )}
      <div
        className={`py-3 px-4 max-w-[75%] text-sm ${
          chat.role === "model"
            ? "bg-lime-100 text-gray-800 rounded-[13px_13px_13px_3px]"
            : "bg-lime-600 text-white rounded-[13px_13px_3px_13px]"
        }`}
      >
        <ReactMarkdown
          components={{
            
            a: ({ href, children }) => (
              <Link
                to={href}
                className={`font-semibold underline transition-colors ${
                  chat.role === "model"
                    ? "text-lime-700 hover:text-lime-900"
                    : "text-lime-200 hover:text-white"
                }`}
              >
                {children}
              </Link>
            ),
            p: ({ children }) => <p className="whitespace-pre-line">{children}</p>
          }}
        >
          {chat.text}
        </ReactMarkdown>
      </div>
    </div>
  );
};

export default ChatMessage;