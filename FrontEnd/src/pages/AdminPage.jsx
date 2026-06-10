import React, { useState, useEffect, useRef } from "react";
import UsersManagement from "./UsersManagement";
import GasStationsManagement from "./GasStationsManagement";

const baseUrl = (
  import.meta.env.VITE_API_URL || "http://localhost:5211"
).trim();

const cleanBaseUrl = baseUrl.endsWith("/") ? baseUrl.slice(0, -1) : baseUrl;
const API_BASE_URL = `${cleanBaseUrl}/api/admin`;

export default function AdminPage() {
  const [activeTab, setActiveTab] = useState("users");
  const [showScrollButton, setShowScrollButton] = useState(false);

  const tabUsersRef = useRef(null);
  const tabGasRef = useRef(null);

  useEffect(() => {
    const handleScroll = () => {
      const scrolled =
        window.scrollY ||
        document.documentElement.scrollTop ||
        document.body.scrollTop;
      if (scrolled > 100) {
        setShowScrollButton(true);
      } else {
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

  const handleKeyDown = (e, currentTab) => {
    if (e.key === "ArrowRight" || e.key === "ArrowLeft") {
      e.preventDefault();
      if (currentTab === "users") {
        setActiveTab("gasStations");
        tabGasRef.current?.focus();
      } else {
        setActiveTab("users");
        tabUsersRef.current?.focus();
      }
    }
  };

  return (
    <main className="p-6 max-w-5xl mx-auto bg-transparent min-h-screen">
      <h1 className="text-3xl md:text-4xl font-extrabold text-white drop-shadow-md mb-8 text-center uppercase tracking-widest underline decoration-lime-400 decoration-2 underline-offset-4">
        Administratoriaus Zona
      </h1>

      <div
        className="flex gap-4 justify-center items-center mb-10 min-h-[60px] flex-wrap"
        role="tablist"
        aria-label="Administratoriaus valdymo kortelės"
      >
        <button
          ref={tabUsersRef}
          type="button"
          role="tab"
          tabIndex={activeTab === "users" ? 0 : -1}
          aria-selected={activeTab === "users"}
          aria-controls="users-panel"
          id="tab-users"
          onClick={() => setActiveTab("users")}
          onKeyDown={(e) => handleKeyDown(e, "users")}
          className={`transition-all duration-300 ease-in-out cursor-pointer text-center rounded-xl p-3 text-base md:text-lg tracking-wide shadow-md focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-4 focus-visible:outline-white ${
            activeTab === "users"
              ? "bg-lime-800 text-white font-extrabold border-2 border-lime-400 ring-2 ring-lime-900"
              : "bg-gray-900/90 text-gray-100 font-bold hover:bg-gray-800 border border-gray-700 opacity-90 hover:opacity-100"
          }`}
        >
          Vartotojų Valdymas
        </button>

        <span
          className="text-gray-300 text-xl font-light hidden sm:inline"
          aria-hidden="true"
        >
          /
        </span>

        {/* DEGALINIŲ VALDYMO MYGTUKAS */}
        <button
          ref={tabGasRef}
          type="button"
          role="tab"
          tabIndex={activeTab === "gasStations" ? 0 : -1}
          aria-selected={activeTab === "gasStations"}
          aria-controls="gas-stations-panel"
          id="tab-gas-stations"
          onClick={() => setActiveTab("gasStations")}
          onKeyDown={(e) => handleKeyDown(e, "gasStations")}
          className={`transition-all duration-300 ease-in-out cursor-pointer text-center rounded-xl p-3 text-base md:text-lg tracking-wide shadow-md focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-4 focus-visible:outline-white ${
            activeTab === "gasStations"
              ? "bg-lime-800 text-white font-extrabold border-2 border-lime-400 ring-2 ring-lime-900"
              : "bg-gray-900/90 text-gray-100 font-bold hover:bg-gray-800 border border-gray-700 opacity-90 hover:opacity-100"
          }`}
        >
          Degalinių Valdymas
        </button>
      </div>

      <div className="focused-panel-container">
        {activeTab === "users" && (
          <div
            id="users-panel"
            role="tabpanel"
            aria-labelledby="tab-users"
            tabIndex={0}
            className="focus:outline-none"
          >
            <UsersManagement apiBaseUrl={API_BASE_URL} />
          </div>
        )}
        {activeTab === "gasStations" && (
          <div
            id="gas-stations-panel"
            role="tabpanel"
            aria-labelledby="tab-gas-stations"
            tabIndex={0}
            className="focus:outline-none"
          >
            <GasStationsManagement apiBaseUrl={API_BASE_URL} />
          </div>
        )}
      </div>

      {/* 🌟 SCROLL BUTTON: Pridėtas aria-label ir focus būsena */}
      <button
        onClick={scrollToTop}
        className={`fixed bottom-6 right-6 z-[9999] bg-lime-800 text-white rounded-full shadow-2xl hover:bg-lime-700 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-white transition-all duration-300 ease-in-out cursor-pointer flex items-center justify-center border-2 border-lime-400 w-14 h-14 ${
          showScrollButton
            ? "opacity-100 scale-100 visible"
            : "opacity-0 scale-50 invisible pointer-events-none"
        }`}
        aria-label="Grįžti į puslapio viršų"
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
          aria-hidden="true"
        >
          <path
            strokeLinecap="round"
            strokeLinejoin="round"
            d="M4.5 10.5 12 3m0 0 7.5 7.5M12 3v18"
          />
        </svg>
      </button>
    </main>
  );
}