import React, { useState, useEffect } from "react";
import UsersManagement from "./UsersManagement";
import GasStationsManagement from "./GasStationsManagement";

const baseUrl = (import.meta.env.VITE_API_URL || "http://localhost:5211").trim();

const cleanBaseUrl = baseUrl.endsWith('/') ? baseUrl.slice(0, -1) : baseUrl;
const API_BASE_URL = `${cleanBaseUrl}/api/admin`;

export default function AdminPage() {
  const [activeTab, setActiveTab] = useState("users");
  const [showScrollButton, setShowScrollButton] = useState(false);

  useEffect(() => {
    const handleScroll = () => {
      const scrolled = window.scrollY || document.documentElement.scrollTop || document.body.scrollTop;
      if (scrolled > 100) {
        setShowScrollButton(true);
      } else {
        setShowScrollButton(false);
      }
    };

    window.addEventListener("scroll", handleScroll, true);
    return () => window.removeEventListener("scroll", handleScroll, true);
  }, []);

  const scrollToTop = () => {
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  return (
    <div className="p-6 max-w-5xl mx-auto bg-transparent min-h-screen">
      {/* ♿ WCAG SPRENDIMAS: text-lime-100 pakeistas į gryną baltą text-white ir pridėtas drop-shadow-md, 
          kad tekstas aiškiai atsiskirtų nuo bet kokio nuotraukos fono */}
      <h1 className="text-3xl font-bold text-white drop-shadow-md mb-8 text-center uppercase tracking-widest underline decoration-lime-400 decoration-2 underline-offset-4">
        Administratoriaus Zona
      </h1>

      {/* Pridėtas role="tablist" ekrano skaitytuvams */}
      <div className="flex gap-6 justify-center items-center mb-10 min-h-[80px]" role="tablist" aria-label="Administratoriaus valdymas">
        
        {/* VARTOTOJŲ VALDYMO MYGTUKAS */}
        <button
          type="button"
          role="tab"
          aria-selected={activeTab === "users"}
          aria-controls="users-panel"
          id="tab-users"
          onClick={() => setActiveTab("users")}
          className={`transition-all duration-500 ease-in-out cursor-pointer text-center rounded-xl font-bold tracking-wide focus-visible:outline-2 focus-visible:outline-offset-4 focus-visible:outline-white ${
            activeTab === "users"
              ? "text-4xl md:text-6xl text-white bg-transparent px-2 py-1 drop-shadow-lg"
              : "text-sm bg-gray-900/80 backdrop-blur-sm text-gray-100 hover:bg-gray-800 px-4 py-2 opacity-80 hover:opacity-100 shadow-md border border-gray-700"
          }`}
        >
          Vartotojų Valdymas
        </button>

        {/* ♿ WCAG SPRENDIMAS: Skirtuko spalva iš text-gray-300 pakeista į text-gray-200, kad matytųsi ant miško fono */}
        <span className="text-gray-200 text-xl font-light" aria-hidden="true">/</span>

        {/* DEGALINIŲ VALDYMO MYGTUKAS */}
        <button
          type="button"
          role="tab"
          aria-selected={activeTab === "gasStations"}
          aria-controls="gas-stations-panel"
          id="tab-gas-stations"
          onClick={() => setActiveTab("gasStations")}
          className={`transition-all duration-500 ease-in-out cursor-pointer text-center rounded-xl font-bold tracking-wide focus-visible:outline-2 focus-visible:outline-offset-4 focus-visible:outline-white ${
            activeTab === "gasStations"
              ? "text-4xl md:text-6xl text-white bg-transparent px-2 py-1 drop-shadow-lg"
              : "text-sm bg-gray-900/80 backdrop-blur-sm text-gray-100 hover:bg-gray-800 px-4 py-2 opacity-80 hover:opacity-100 shadow-md border border-gray-700"
          }`}
        >
          Degalinių Valdymas
        </button>
      </div>

      {/* Lentelių atvaizdavimas su pritaikytais ID ekrano skaitytuvams */}
      <div className="transition-all duration-500">
        {activeTab === "users" && (
          <div id="users-panel" role="tabpanel" aria-labelledby="tab-users">
            <UsersManagement apiBaseUrl={API_BASE_URL} />
          </div>
        )}
        {activeTab === "gasStations" && (
          <div id="gas-stations-panel" role="tabpanel" aria-labelledby="tab-gas-stations">
            <GasStationsManagement apiBaseUrl={API_BASE_URL} />
          </div>
        )}
      </div>

      {/* 🌟 SCROLL BUTTON: Pridėtas aria-label ir focus būsena */}
      <button
        onClick={scrollToTop}
        className={`fixed bottom-6 left-1/2 -translate-x-1/2 z-[9999] bg-lime-800 text-white p-4 rounded-full shadow-2xl hover:bg-lime-700 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-white transition-all duration-500 ease-in-out cursor-pointer flex items-center justify-center border-2 border-lime-400 w-14 h-14 ${
          showScrollButton 
            ? "opacity-100 scale-100 visible" 
            : "opacity-0 scale-50 invisible pointer-events-none"
        }`}
        aria-label="Grįžti į puslapio viršų"
        title="Į viršų"
      >
        <svg 
          xmlns="http://www.w3.org/2000/svg" 
          fill="none" 
          viewBox="0 0 24 24" 
          strokeWidth="3" 
          stroke="currentColor" 
          className="w-6 h-6 text-white block"
          aria-hidden="true"
        >
          <path strokeLinecap="round" strokeLinejoin="round" d="M4.5 10.5 12 3m0 0 7.5 7.5M12 3v18" />
        </svg>
      </button>
    </div>
  );
}