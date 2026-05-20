import React, { useState } from "react";
import { motion, AnimatePresence } from "framer-motion";
import LoginPopup from "./LoginPopup";
import engineStartSound from "../assets/animations/engineStart.mp3";
import routeScreen from "../assets/images/route.png";
import { login } from "../services/authService.js";

const HomePage = () => {
  const [isStarted, setIsStarted] = useState(false);
  const [showLogin, setShowLogin] = useState(false);
  const [user, setUser] = useState(null);

  const handleIgnition = () => {
    if (isStarted) return;

    const audio = new Audio(engineStartSound);
    audio.play().catch((err) => console.error("Audio play error:", err));

    setIsStarted(true);

    setTimeout(() => {
      setShowLogin(true);
    }, 1500);
  };

  const textVariants = {
    hidden: { opacity: 0, y: 20 },
    visible: (i) => ({
      opacity: 1,
      y: 0,
      transition: { delay: i * 0.6, duration: 0.95 },
    }),
  };

  const ulVariants = {
    hidden: { opacity: 0, x: 40 },
    visible: (e) => ({
      opacity: 1,
      x: 0,
      transition: { delay: e * 0.95, duration: 0.95 },
    }),
  };

  return (
    <main className="min-h-screen flex flex-col items-center justify-center text-white overflow-hidden p-6">
      <div className="home__data ">
        <div className="flex flex-col items-center justify-center p-6">
          <h1 className="flex items-center justify-center py-4 px-2 font-bold text-4xl">
            {user ? `Labas, ${user.userName}! ` : "Labas, Nepažįstamasis! "}
          </h1>
          <p className="font-bold text-2xl p-2">ką galėsi daryti prisiregistravęs:</p>
          {["Suplanuoti maršrutą", "Įterpti degalinę pakeliui", "Nusisiųsti maršrutą į Google Maps"].map(
            (text, e) => (
              <motion.p
                key={e}
                custom={e}
                variants={ulVariants}
                initial="hidden"
                animate="visible"
                className="text-2xl font-semibold"
              >
                {text}
              </motion.p>
            ),
          )}
          <div className="flex p-4">
            <img
              src={routeScreen}
              alt=""
              className="w-80 h-60 md:w-180 md:h-160 rounded-2xl hover:shadow-2xl hover:scale-110 transition-all duration-700"
            />
          </div>
        </div>
      </div>

      {/* Introduction */}
      <div className="text-center mb-16">
        {["CTRL ALT DELETE", "Pristato projektą"].map((text, i) => (
          <motion.h2
            key={i}
            custom={i}
            variants={textVariants}
            initial="hidden"
            animate="visible"
            className="text-3xl md:text-5xl font-extrabold tracking-widest mb-4 uppercase italic opacity-90"
          >
            {text}
          </motion.h2>
        ))}
      </div>

      {/* start/Stop button */}
      <div className="flex items-center justify-center">
        <motion.button
          onMouseDown={handleIgnition}
          whileHover={{ scale: 1.05 }}
          whileTap={{ scale: 0.95 }}
          className={`
            relative w-40 h-40 md:w-56 md:h-56 rounded-full border-8 
            flex flex-col items-center justify-center transition-all duration-700
            ${
              isStarted
                ? "bg-green-600 border-green-400 shadow-[0_0_50px_rgba(34,197,94,0.8)]"
                : "bg-red-700 border-red-900 shadow-[0_0_30px_rgba(185,28,28,0.4)] hover:shadow-[0_0_50px_rgba(185,28,28,0.6)]"
            }
          `}
        >
          {/* Button */}
          <span className="text-xs md:text-sm font-bold tracking-tighter opacity-70 mb-1">
            Start
          </span>
          <span className="text-2xl md:text-4xl font-black tracking-tight">
            YOUR
          </span>
          <span className="text-xs md:text-sm font-bold tracking-tighter opacity-70 mt-1 uppercase">
            trip
          </span>

          {/* animation */}
          {!isStarted && (
            <motion.div
              animate={{ opacity: [0.3, 0.6, 0.3] }}
              transition={{ repeat: Infinity, duration: 2 }}
              className="absolute inset-0 rounded-full border-2 border-white/20"
            />
          )}
        </motion.button>
      </div>

      {/* login Popup */}
      <AnimatePresence>
        {showLogin && (
          <LoginPopup
            show={showLogin}
            onClose={() => {
              setShowLogin(false);
              setIsStarted(false);
            }}
            onLoginSuccess={(userData) => setUser(userData)}
          />
        )}
      </AnimatePresence>
    </main>
  );
};

export default HomePage;
