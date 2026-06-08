import React, { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { GiHamburgerMenu } from "react-icons/gi";
import {Logo} from "./Logo.jsx";

const Navbar = ({ handleLoginPopup, user, setUser }) => {
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.removeItem("app_user");
    setUser(null);
    navigate("/");
  };

  return (
    <nav className="shadow-lg bg-white/30 backdrop-blur-sm sticky top-0 z-1001">
      <div className="container mx-auto flex justify-between items-center p-4 gap-4">
        
        {/* LOGO  */}
        <Link 
          to="/" 
          state={{ fromNavigation: true }} 
          className="text-gray-800 text-2xl sm:text-3xl font-bold min-w-max"
        >
          <Logo />
        </Link>

        {/* --- DESKTOP MENIU  --- */}
        <div className="hidden md:block">
          <ul className="flex font-semibold items-center gap-2 lg:gap-4 text-sm lg:text-base">
            <li>
              <Link 
                to="/" 
                state={{ fromNavigation: true }} 
                className="text-gray-700 hover:text-gray-900 hover:bg-lime-400 hover:rounded-2xl hover:transition-colors px-3 py-2 select-none block whitespace-nowrap"
              >
                Į pradžią
              </Link>
            </li>

            {user && (
              <li>
                <Link 
                  to="/MapPage" 
                  className="text-gray-700 hover:text-gray-900 hover:bg-lime-400 hover:rounded-2xl hover:transition-colors px-3 py-2 select-none block whitespace-nowrap"
                >
                  Žemėlapis
                </Link>
              </li>
            )}

            {user && (
              <li>
                <Link 
                  to="/SavedRoutesPage" 
                  className="text-gray-700 hover:text-gray-900 hover:bg-lime-400 hover:rounded-2xl hover:transition-colors px-3 py-2 select-none block whitespace-nowrap"
                >
                  Maršrutų istorija
                </Link>
              </li>
            )}

            <li>
              <Link 
                to="/ContactPage" 
                className="text-gray-700 hover:text-gray-900 hover:bg-lime-400 hover:rounded-2xl hover:transition-colors px-3 py-2 select-none block whitespace-nowrap"
              >
                Kontaktai
              </Link>
            </li>

            <li>
              {user ? (
                <div className="flex items-center gap-2 bg-white/50 px-3 py-1.5 rounded-2xl border border-white/30 max-w-xs shadow-sm">
                  <span className="text-gray-800 font-bold text-xs lg:text-sm truncate max-w-37.5 lg:max-w-50">
                    {user.username.split('@')[0]}
                  </span>
                  <button
                    onClick={handleLogout}
                    className="bg-red-500 hover:bg-red-600 text-white text-[11px] font-bold px-2 py-1 rounded-xl transition-all cursor-pointer whitespace-nowrap"
                  >
                    Atsijungti
                  </button>
                </div>
              ) : (
                <button
                  onClick={handleLoginPopup}
                  className="text-gray-700 hover:text-gray-900 hover:bg-lime-400 hover:rounded-2xl hover:transition-colors px-4 py-2 select-none cursor-pointer whitespace-nowrap"
                >
                  Prisijunk
                </button>
              )}
            </li>
          </ul>
        </div>

        {/* Hamburger mygtukas  */}
        <GiHamburgerMenu 
          className="flex items-center text-3xl md:hidden cursor-pointer text-gray-800 ml-auto" 
          onClick={() => setIsMenuOpen(!isMenuOpen)}
        />

        {/* --- MOBILE MENIU  --- */}
        <div 
          className={`absolute md:hidden top-full left-0 w-full bg-white/95 backdrop-blur-md flex flex-col items-center gap-4 font-semibold text-lg border-t border-gray-100 shadow-xl transform transition-all duration-300 ${isMenuOpen ? "opacity-100 py-6" : "opacity-0 pointer-events-none h-0 overflow-hidden"}`} 
        >
          <Link to="/" className="w-full text-center p-3 hover:bg-lime-400 hover:text-white transition-all" onClick={() => setIsMenuOpen(false)}>Į pradžią</Link>
          
          {user && (
              
                <Link 
                  to="/MapPage" 
                  className="text-gray-700 hover:text-gray-900 hover:bg-lime-400 hover:rounded-2xl hover:transition-colors px-3 py-2 select-none block whitespace-nowrap"
                >
                  Žemėlapis
                </Link>
              
            )}

            {user && (
              
                <Link 
                  to="/SavedRoutesPage" 
                  className="text-gray-700 hover:text-gray-900 hover:bg-lime-400 hover:rounded-2xl hover:transition-colors px-3 py-2 select-none block whitespace-nowrap"
                >
                  Maršrutų istorija
                </Link>
             
            )}

          <Link to="/ContactPage" className="w-full text-center p-3 hover:bg-lime-400 hover:text-white transition-all" onClick={() => setIsMenuOpen(false)}>Kontaktai</Link>
        </div>
        
        {/* --- MOBILE LOGIN  --- */}
        <div className="block md:hidden min-w-max">
          {user ? (
            <div className="flex flex-col items-end gap-0.5 px-2">
              <span className="text-gray-800 text-xs font-bold truncate max-w-30">
                {user.userName}
              </span>
              <button
                onClick={handleLogout}
                className="text-red-600 text-[11px] font-bold underline hover:text-red-700"
              >
                Atsijungti
              </button>
            </div>
          ) : (
            <button
              onClick={handleLoginPopup}
              className="text-gray-700 text-lg font-semibold hover:text-gray-900 px-2 cursor-pointer"
            >
              Prisijunk
            </button>
          )}
        </div>
        
      </div>
    </nav>
  );
};

export default Navbar;