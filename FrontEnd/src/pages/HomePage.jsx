import { motion } from "framer-motion";

import routeScreen from "../assets/images/route.png";
import ChatBot from "../assets/images/ChatBot.png";
import filters from "../assets/images/filters.png";
import fuelPrice from "../assets/images/fuelPrice.png";

import Carousel from "../components/common/Carousel.jsx";

const slides = [
  routeScreen,
  ChatBot,
  fuelPrice,
  filters,
];

const HomePage = ({ user }) => {
  const textVariants = {
    hidden: {
      opacity: 0,
      y: 20,
    },

    visible: (index) => ({
      opacity: 1,
      y: 0,
      transition: {
        delay: index * 0.6,
        duration: 0.95,
      },
    }),
  };

  const listItemVariants = {
    hidden: {
      opacity: 0,
      x: 40,
    },

    visible: (index) => ({
      opacity: 1,
      x: 0,
      transition: {
        delay: index * 0.95,
        duration: 0.95,
      },
    }),
  };

  const username = user?.username
    ? user.username.split("@")[0]
    : null;

  return (
    <main className="flex min-h-full flex-col items-center justify-center overflow-hidden p-2 text-white">
      <div className="mb-8 text-center">
        {[
          "CTRL ALT DELETE",
          "Pristato projektą",
        ].map((text, index) => (
          <motion.h2
            key={text}
            custom={index}
            variants={textVariants}
            initial="hidden"
            animate="visible"
            className="
              mb-4 text-3xl font-extrabold uppercase
              italic tracking-widest opacity-90
              text-shadow-[0_2px_8px_rgba(0,0,0,0.9)]
              md:text-5xl
            "
          >
            {text}
          </motion.h2>
        ))}
      </div>

      <div className="home__data">
        <div className="flex flex-col items-center justify-center p-4">
          <h1 className="flex items-center justify-center px-2 py-4 text-4xl font-bold text-lime-800 drop-shadow-xl">
            {username
              ? `Labas, ${username}!`
              : "Labas, Nepažįstamasis!"}
          </h1>

          <p className="p-2 text-2xl font-bold text-shadow-[0_2px_8px_rgba(0,0,0,0.9)]">
            Ką galėsi daryti prisiregistravęs:
          </p>

          {[
            "Suplanuoti maršrutą",
            "Įterpti degalinę pakeliui",
            "Nusiųsti maršrutą į Google Maps",
            "Filtruoti pagal reikiamą kurą",
            "Pasirinkti pigiausią reikiamo kuro degalinę",
            "Pasinaudoti ChatBot pagalba",
          ].map((text, index) => (
            <motion.p
              key={text}
              custom={index}
              variants={listItemVariants}
              initial="hidden"
              animate="visible"
              className="text-2xl font-semibold text-shadow-[0_2px_8px_rgba(0,0,0,0.9)]"
            >
              {text}
            </motion.p>
          ))}

          <div className="mt-4 max-w-lg rounded-2xl">
            <Carousel
              autoSlide
              autoSlideInterval={3000}
            >
              {slides.map((slide, index) => (
                <img
                  key={slide}
                  src={slide}
                  alt={`Application feature ${index + 1}`}
                  className="flex h-auto w-50 shrink-0 rounded-2xl object-cover"
                />
              ))}
            </Carousel>
          </div>
        </div>
      </div>
    </main>
  );
};

export default HomePage;