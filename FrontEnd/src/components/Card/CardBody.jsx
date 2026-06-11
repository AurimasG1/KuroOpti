import React from "react";

const CardBody = ({ children, className = "" }) => {
  return <div className={`${className} text-white text-shadow-[0_2px_8px_rgba(0,0,0,0.9)] `}>{children}</div>;
};

export default CardBody;