import React from "react";
// import logo from "../../assets/images/logo.png";
// import greenlogo from "../../assets/images/greenlogo.png";
import darkgreen from "../../assets/images/darkgreen.png";

export const Logo = () => {
  return (
    <div>
      <img
        src={darkgreen}
        alt="CtrlAltDeleteLogo"
        className="h-full object-contain motion-safe:animate-spin" 
                style={{ animationDuration: '10s' }}
      />
    </div>
  );
};
