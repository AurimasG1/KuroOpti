import React from "react";
import { Link } from "react-router-dom";
import { useState } from "react";
import { GiHamburgerMenu } from "react-icons/gi";


const Navbar = ({ handleLoginPopup }) => {

  const [isMenuOpen, setIsMenuOpen] = useState(false);

  return (
    <nav className="shadow-lg bg-white/30 backdrop-blur-sm sticky top-0 z-1001">
      <div className="container mx-auto flex justify-between items-center p-4">
        
        <Link 
          to="/" 
          state={{ fromNavigation: true }} 
          className="text-gray-800 text-3xl font-bold"
        >
          Logo
        </Link>

        <div className="hidden sm:block">
          <ul className="flex font-semibold items-center gap-4">
            <li>
              <Link 
                to="/" 
                state={{ fromNavigation: true }} 
                className="text-gray-700 hover:text-gray-900 hover:bg-lime-400 hover:rounded-2xl hover:transition-colors px-4 py-2 select-none"
              >
                Į pradžią
              </Link>
            </li>
            <li>
              <Link 
                to="/MapPage" 
                className="text-gray-700 hover:text-gray-900 hover:bg-lime-400 hover:rounded-2xl hover:transition-colors px-4 py-2 select-none"
              >
                Žemėlapis
              </Link>
            </li>
            <li>
              <Link 
                to="/ContactPage" 
                className="text-gray-700 hover:text-gray-900 hover:bg-lime-400 hover:rounded-2xl hover:transition-colors px-4 py-2 select-none"
              >
                Kontaktai
              </Link>
            </li>
            <li>
              <button
                onClick={handleLoginPopup}
                className="text-gray-700 hover:text-gray-900 hover:bg-lime-400 hover:rounded-2xl hover:transition-colors px-4 py-2 select-none"
                >
                Prisijunk
              </button>
            </li>
          </ul>
        </div>

        {/* Hamburger / Mobile Login */}

        <GiHamburgerMenu className="flex items-center text-3xl bx bx-menu sm:hidden cursor-pointer" onClick={() => setIsMenuOpen(!isMenuOpen)}/>

          <div className={`absolute sm:hidden top-24 left-0 w-full bg-white/30 backdrop-blur-sm flex flex-col items-center gap-6 font-semibold text-lg transform transition-transform ${isMenuOpen ? "opacity-100" : "opacity-0"}`} style={{transition: "transform 0.3s ease, opacity 0.3s ease"}}>
            <Link 
                to="/MapPage"  className="list-none w-full text-center p-4 hover:bg-lime-400 hover:text-white transition-all cursor-pointer">Žemėlapis</Link>
            <Link 
                to="/ContactPage"  className="list-none w-full text-center p-4 hover:bg-lime-400 hover:text-white transition-all cursor-pointer">Kontaktai</Link>
          </div>
        
        <div className="block sm:hidden">
          <button
            onClick={handleLoginPopup}
            className="text-gray-700 text-xl font-semibold hover:text-gray-900 px-4"
          >
            Prisijunk
          </button>
        </div>

        
      </div>
    </nav>
  );
};

export default Navbar;