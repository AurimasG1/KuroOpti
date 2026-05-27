import React, { useEffect, useRef, useState } from "react";
import ChatBotIcon from "../components/ChatBot/ChatBotIcon";
import { IoArrowDownCircleOutline } from "react-icons/io5";
import ChatForm from "../components/ChatBot/ChatForm";
import ChatMessage from "../components/ChatBot/ChatMessage";
import { IoChatboxOutline } from "react-icons/io5";
import { MdClose } from "react-icons/md";
import { Link } from 'react-router-dom'; 

const INITIAL_THEMES = [
  { id: "intro", label: "Prisistatymas" },
  { id: "team", label: "Komanda" },
  { id: "features", label: "Ką šioje programėlėje galite?" },
];

const FEATURE_THEMES = [
  { id: "create_route", label: "Sukurti savo maršrutą" },
  { id: "add_gas", label: "Pridėti degalinę į maršrutą" },
  { id: "remove_gas", label: "Pašalinti degalinę" },
  { id: "send_route", label: "Siųsti maršrutą į..." },
  { id: "back", label: "⬅ Grįžti atgal", isBack: true },
];

const STATIC_RESPONSES = {
  intro: "Ši programėlė yra skirta išmaniam kelionių ir maršrutų planavimui. Čia galite susidėlioti kelionės taškus, pasitikrinti kuro kainą pakeliui ir pridėti degalines į maršrutą!",
  team: "Mūsų komandą sudaro entuziastingi programuotojai. Visus kontaktus ir el. paštus galite rasti čia: [Kontaktų puslapis](/ContactPage)",
  create_route: "Norėdami sukurti maršrutą:\n1. Eikite į žemėlapio langą.\n2. Viršuje esančioje paieškoje įveskite tikslą.\n3. Spauskite mygtuką 'Rodyti maršrutą'.",
  add_gas: "Norėdami pridėti degalinę:\n1. Jau sukurtame maršrute spustelėkite mygtuką 'Pridėti degalinę'.\n2. Pasirinkite tinkamą degalinę iš sąrašo ar žemėlapio.\n3. ",
  remove_gas: "Norėdami pašalinti degalinę:\n1. Maršruto taškų sąraše raskite degalinę, kurios nebereikia.\n2. Spauskite šalia esančią 'X' .",
  send_route: "Maršruto siuntimas:\n1. Ekrano apačioje spauskite \n2.  'Pradėti maršrutą' būsite nukreipti į Google Maps."
};

const ChatBotPage = () => {
  const [chatHistory, setChatHistory] = useState([]);
  const [showChatBot, setShowChatBot] = useState(false);
  const [currentOptions, setCurrentOptions] = useState(INITIAL_THEMES);
  const chatBodyRef = useRef();

  const generateBotResponse = async (history) => {
    const updateHistory = (text, isError = false) => {
      setChatHistory((prev) => [
        ...prev.filter((msg) => msg.text !== "Thinking..."),
        { role: "model", text, isError },
      ]);
    };

    const cleanHistory = history.filter((msg) => msg.text !== "Thinking...");
    const formattedHistory = cleanHistory.map(({ role, text }) => ({ 
      role, 
      parts: [{ text }] 
    }));

    const requestOptions = {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ contents: formattedHistory }),
    };

    try {
      const response = await fetch(import.meta.env.VITE_API_URL, requestOptions);
      const textData = await response.text();
      
      if (!textData) {
        throw new Error("Serveris grąžino tuščią atsakymą.");
      }

      const data = JSON.parse(textData);
      if (!response.ok) throw new Error(data.error?.message || "Something went wrong!");

      const apiResponseText = data.candidates[0].content.parts[0].text.trim();
      updateHistory(apiResponseText);
    } catch (error) {
      updateHistory(error.message, true);
    }
  };

  const handleOptionClick = async (option) => {
    if (option.isBack) {
      setCurrentOptions(INITIAL_THEMES);
      return;
    }

    const updatedHistory = [...chatHistory, { role: "user", text: option.label }];
    setChatHistory(updatedHistory);

    if (option.id === "features") {
      setChatHistory((prev) => [
        ...prev,
        { role: "model", text: "Štai ką galite padaryti programėlėje. Pasirinkite funkciją, kad sužinotumėte daugiau:" }
      ]);
      setCurrentOptions(FEATURE_THEMES);
    } 
    else if (STATIC_RESPONSES[option.id]) {
      setChatHistory((prev) => [
        ...prev,
        { role: "model", text: STATIC_RESPONSES[option.id] }
      ]);
      setCurrentOptions(INITIAL_THEMES);
    } 
    else {
      setChatHistory((prev) => [...prev, { role: "model", text: "Thinking..." }]);
      await generateBotResponse(updatedHistory);
      setCurrentOptions(INITIAL_THEMES);
    }
  };

  useEffect(() => {
    if (chatBodyRef.current) {
      chatBodyRef.current.scrollTo({
        top: chatBodyRef.current.scrollHeight,
        behavior: "smooth",
      });
    }
  }, [chatHistory]);

  return (
    <div className="relative">
      {/* Bot mygtukas ekrano kampe */}
      <button
        onClick={() => setShowChatBot((prev) => !prev)}
        className="fixed bottom-7.5 right-9 border-none h-12.5 w-12.5 flex cursor-pointer rounded-full items-center justify-center bg-lime-600 shadow-lg text-white z-1001"
      >
        {!showChatBot ? (
          <IoChatboxOutline className="text-2xl transition-all" />
        ) : (
          <MdClose className="text-2xl transition-all" />
        )}
      </button>

      {/* Chat langas */}
      <div
        className={`fixed bottom-0 right-0 w-full h-full sm:bottom-22.5 sm:right-9 sm:w-105 sm:h-auto sm:max-h-150 bg-white sm:rounded-2xl shadow-2xl overflow-hidden transition-all duration-300 origin-bottom-right z-1001 ${
          showChatBot
            ? "opacity-100 scale-100 pointer-events-auto"
            : "opacity-0 scale-95 pointer-events-none"
        }`}
      >
        {/* Antraštė */}
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

        {/* Žinučių istorijos langas */}
        <div
          ref={chatBodyRef}
          className="flex flex-col gap-5 h-[calc(100vh-220px)] sm:h-90 overflow-y-auto py-6 px-5.5 mb-32 [scrollbar-width:thin] [scrollbar-color:#65a30d_transparent]"
        >
          {/* Pradinė pasisveikinimo žinutė */}
          <div className="flex gap-2.75 items-center">
            <ChatBotIcon className="h-8.75 w-8.75 p-1.5 fill-white bg-lime-600 rounded-full" />
            <p className="py-3 px-4 max-w-[75%] bg-lime-100 wrap-break-words text-sm rounded-[13px_13px_13px_3px] text-gray-800">
              Sveiki, <br /> Kuo galiu padėti?
            </p>
          </div>

          {chatHistory.map((chat, index) => (
            <ChatMessage key={index} chat={chat} />
          ))}
        </div>

        {/* Apatinė dalis: Mygtukai + Įvesties laukas */}
        <div className="absolute bottom-0 w-full bg-white pt-2 pb-5.5 px-4 border-t border-gray-100 flex flex-col gap-3">
          {/* Dinaminiai pasirinkimo mygtukai */}
          <div className="flex flex-wrap gap-2 max-h-24 overflow-y-auto pt-1">
            {currentOptions.map((option) => (
              <button
                key={option.id}
                onClick={() => handleOptionClick(option)}
                className={`text-xs py-1.5 px-3 rounded-full border transition-all cursor-pointer ${
                  option.isBack
                    ? "border-gray-300 bg-gray-50 text-gray-600 hover:bg-gray-100"
                    : "border-lime-600 bg-white text-lime-700 hover:bg-lime-50 font-medium"
                }`}
              >
                {option.label}
              </button>
            ))}
          </div>

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