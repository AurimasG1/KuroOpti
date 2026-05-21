import React from "react";

const CardBody = ({ children, className = "" }) => {
  return <div className={`text-white text-shadow-lg ${className}`}>{children}</div>;
};

export default CardBody;
