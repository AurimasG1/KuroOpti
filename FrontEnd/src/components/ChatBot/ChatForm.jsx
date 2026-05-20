import React, { useRef } from "react";
import { FiArrowUpCircle } from "react-icons/fi";

const ChatForm = ({ chatHistory, setChatHistory, generateBotResponse }) => {
  const inputRef = useRef();
  const handleFormSubmit = (e) => {
    e.preventDefault();
    const userMessage = inputRef.current.value.trim();
    if (!userMessage) return;
    inputRef.current.value = "";

    //Update chat history with user's message
    setChatHistory((history) => [
      ...history,
      { role: "user", text: userMessage },
    ]);

    //Delay 600 ms before showing "Thinking..." and generate response
    setTimeout(() => {
      //Add a "Thinking..." placeholder for the bot's response
      setChatHistory((history) => [
        ...history,
        { role: "model", text: "Thinking..." },
      ]);
      //Call the function to generate the bot's response
      generateBotResponse([
        ...chatHistory,
        { role: "user", text: userMessage },
      ]);
    }, 600);
  };

  return (
    <form
      className="flex w-full items-center bg-white rounded-full p-1.5 pl-4 border mx-6 border-gray-200 focus-within:border-lime-500 focus-within:ring-2 focus-within:ring-lime-100 transition-all shadow-md"
      onSubmit={handleFormSubmit}
    >
      <input
        ref={inputRef}
        type="text"
        placeholder="Message..."
        className="w-full h-11 text-sm text-gray-800 bg-transparent border-none outline-none placeholder-gray-400"
        required
      />
      <button
        type="submit"
        className="h-9 w-9 text-white shrink-0 rounded-full bg-lime-500 hover:bg-lime-600 transition-colors flex items-center justify-center text-2xl cursor-pointer"
      >
        <FiArrowUpCircle />
      </button>
    </form>
  );
};

export default ChatForm;
