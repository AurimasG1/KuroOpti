import React, { useState } from "react";
import { motion, AnimatePresence } from "framer-motion";
import LoginPopup from "./LoginPopup";
import routeScreen from "../assets/images/route.png";
import ChatBot from "../assets/images/ChatBot.png";
import filters from "../assets/images/filters.png";
import fuelPrice from "../assets/images/fuelPrice.png";
import { login } from "../services/authService.js";
import Carousel from "../components/common/Carousel.jsx";


const slides =[
  routeScreen,
  ChatBot,
  fuelPrice,
  filters,
]

const HomePage = ({ user: propsUser }) => {
  const [isStarted, setIsStarted] = useState(false);
  const [showLogin, setShowLogin] = useState(false);
  // const [user, setUser] = useState(null);

  const user = propsUser;

  console.log("DABARTINIS USER STATE HOMEPAGE KOMPONENTE:", user);

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
    <main className="min-h-full flex flex-col items-center justify-center text-white overflow-hidden p-2">
       {/* Introduction */}
      <div className="text-center mb-8">
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
        {/* What's done */}
      <div className="home__data ">
        <div className="flex flex-col items-center justify-center p-4">
          <h1 className="flex items-center justify-center py-4 px-2 font-bold text-4xl text-lime-800">
            {user && user.username ? `Labas, ${user.username.split('@')[0]}! ` : "Labas, Nepažįstamasis! "}
          </h1>
          <p className="font-bold text-2xl p-2">
            ką galėsi daryti prisiregistravęs:
          </p>
          {[
            "Suplanuoti maršrutą",
            "Įterpti degalinę pakeliui",
            "Nusisiųsti maršrutą į Google Maps",
            "Filtruoti pagal reikiamą kurą",
            "Pasirinkti pigiausią reikiamo kuro degalinę",
            "Pasinaudok ChatBot pagalba!"
          ].map((text, e) => (
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
          ))}
          <div className="max-w-lg rounded-2xl mt-4">
            <Carousel autoSlide={true} autoSlideInterval={3000}>
              {slides.map((s, index)=>(
                <img 
                  key={index} 
                  src={s} 
                  alt={`Slide ${index}`} 
                  className="w-50 h-auto flex shrink-0 object-cover rounded-2xl"
                />
              ))}
            </Carousel>
          </div>
        </div>
      </div>

     

      {/* login Popup */}
      <AnimatePresence>
        {showLogin && (
          <LoginPopup
            show={loginPopup}
            onClose={handleLoginPopup}
            onLoginSuccess={(userData) => {
              console.log(
                "Vartotojas sėkmingai išsaugotas App state:",
                userData,
              );
              onLoginSuccess(userData);
            }}
          />
        )}
      </AnimatePresence>
    </main>
  );
};

export default HomePage;
