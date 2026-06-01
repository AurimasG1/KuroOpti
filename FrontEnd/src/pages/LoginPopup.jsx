import React, { useState, useRef, useEffect } from "react";
import Signup from "../components/auth/SignupForm.jsx";
import Login from "../components/auth/LoginForm.jsx";
import { motion } from "framer-motion";

const LoginPopup = ({ show, onClose, onLoginSuccess }) => {
  const [showSignup, setShowSignup] = useState(false);
  const loginPopupRef = useRef();

  const handleSignIn = () => {
    setShowSignup(!showSignup);
  };

  useEffect(() => {
    const handleClickOutside = (e) => {
      if (loginPopupRef.current && e.target === loginPopupRef.current) {
        onClose();
      }
    };

    if (show) {
      window.addEventListener("click", handleClickOutside);
    }

    return () => {
      window.removeEventListener("click", handleClickOutside);
    };
  }, [show, onClose]);

  if (!show) return null; 

  return (
    <div
      ref={loginPopupRef}
      className="fixed inset-0 z-1000 w-full h-full bg-black/40 backdrop-blur-sm flex items-center justify-center"
    >
      <div className="relative w-[90%] sm:w-auto">
        <motion.div
          initial={{ opacity: 0, scale: 0.8 }}
          animate={{ opacity: 1, scale: 1 }}
          exit={{ opacity: 0, scale: 0.8 }}
          className="relative rounded-2xl bg-white/10 backdrop-blur-xl border border-white/20 shadow-2xl p-4 sm:w-150 md:w-95"
        >
          {showSignup ? (
            <Signup handleSignIn={handleSignIn} />
          ) : ( 
            <Login 
              handleSignIn={handleSignIn} 
              onLoginSuccess={(userData) => {
                onLoginSuccess(userData);
                onClose();
              }}
            />
          )}
          
          <button 
            onClick={onClose}
            className="absolute top-2 right-2 text-gray-500 hover:text-black p-2"
          >
            ✕
          </button>
        </motion.div>
      </div> 
    </div>
  );
};

export default LoginPopup;