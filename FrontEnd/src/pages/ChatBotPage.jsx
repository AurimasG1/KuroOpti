import React, { useEffect, useRef, useState } from "react";
import ChatBotIcon from "../components/ChatBot/ChatBotIcon";
import { IoArrowDownCircleOutline } from "react-icons/io5";
import { FiArrowUpCircle } from "react-icons/fi";
import ChatForm from "../components/ChatBot/ChatForm";
import ChatMessage from "../components/ChatBot/ChatMessage";
import { IoChatboxOutline } from "react-icons/io5";
import { MdClose } from "react-icons/md";

const ChatBotPage = () => {
  const [chatHistory, setChatHistory] = useState([]);
  const [showChatBot, setShowChatBot] = useState(false);
  const chatBodyRef = useRef()

  const generateBotResponse = async (history) => {
    //Helper function to update chat history
    const updateHistory = (text, isError = false) => {
      setChatHistory((prev) => [
        ...prev.filter((msg) => msg.text !== "Thinking..."),
        { role: "model", text, isError },
      ]);
    };

    //Format chat history for API request
    history = history.map(({ role, text }) => ({ role, parts: [{ text }] }));
    const requestOptions = {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ contents: history }),
    };

    try {
      const response = await fetch(
        import.meta.env.VITE_API_URL,
        requestOptions,
      );
      const data = await response.json();
      if (!response.ok)
        throw new Error(data.error.message || "Something went wrong!");

      //Clean and update chat history with bot's response
      const apiResponseText = data.candidates[0].content.parts[0].text
        .replace(/\*\*(.*?)\*\*/g, "$1")
        .trim();
      updateHistory(apiResponseText);
    } catch (error) {
        updateHistory(error.message, true);
    }
  };

  useEffect(() => {
    //Auto-scroll whenever chat history updates
    chatBodyRef.current.scrollTo({top: chatBodyRef.current.scrollHeight, behavior: "smooth"});
  }, [chatHistory]);


  return (
    <div className="relative">
      <button 
        onClick={() => setShowChatBot(prev => !prev)} 
        className="fixed bottom-7.5 right-9 border-none h-12.5 w-12.5 flex cursor-pointer rounded-full items-center justify-center bg-lime-600 shadow-lg text-white z-1001"
      >
       
        {!showChatBot ? (
          <IoChatboxOutline className="text-2xl transition-all" />
        ) : (
          <MdClose className="text-2xl transition-all" />
        )}
      </button>

      <div className={`fixed bottom-0 right-0 w-full h-full sm:bottom-22.5 sm:right-9 sm:w-105 sm:h-auto sm:max-h-150 bg-white sm:rounded-2xl shadow-2xl overflow-hidden transition-all duration-300 origin-bottom-right z-1001 ${
  showChatBot 
    ? "opacity-100 scale-100 pointer-events-auto" 
    : "opacity-0 scale-95 pointer-events-none"
}`}>

        {/* ChatBot Header */}
        <div className="flex flex-row justify-between bg-lime-600 py-6 px-4">
          <div className="flex gap-2.5 items-center">
            <ChatBotIcon className="h-8.75 w-8.75 p-1.5 fill-lime-600 bg-white rounded-full " />
            <h2 className="text-white text-2xl font-semibold">ChatBot</h2>
          </div>
          <button 
            onClick={() => setShowChatBot(false)} 
            className="h-10 w-10 outline-none bg-none hover:bg-lime-700 transition-all text-3xl pt-0.5 -mr-2.5 text-white rounded-full cursor-pointer flex items-center justify-center"
          >
            <IoArrowDownCircleOutline />
          </button>
        </div>

        {/* ChatBot body */}
        <div 
          ref={chatBodyRef} 
          className="flex flex-col gap-5 h-[calc(100vh-140px)] sm:h-115 overflow-y-auto py-6 px-5.5 mb-20.5 [scrollbar-width:thin] [scrollbar-color:#65a30d_transparent]"
        >
          <div className="flex gap-2.75 items-center">
            <ChatBotIcon className="h-8.75 w8.75 p-1.5 fill-white bg-lime-600 rounded-full" />
            <p className="py-3 px-4 max-w-[75%] bg-lime-100 wrap-break-words text-sm rounded-[13px_13px_13px_3px] text-gray-800">
              Sveiki, <br /> Kuo galiu padėti?
            </p>
          </div>

          {chatHistory.map((chat, index) => (
            <ChatMessage key={index} chat={chat} />
          ))}
        </div>

        {/* ChatBot footer */}
        <div className="flex absolute bottom-0 w-full bg-white pt-4 pb-5.5 pr-5 rounded-t-3xl border-t border-gray-100">
          <ChatForm
            chatHistory={chatHistory}
            setChatHistory={setChatHistory}
            generateBotResponse={generateBotResponse}
          />
        </div>
      </div>
    </div>
  );
};

export default ChatBotPage;
