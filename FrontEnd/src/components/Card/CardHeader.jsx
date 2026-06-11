import React from "react";

const CardHeader = ({ children, className = "" }) => {
    return <div className={`mb-4 ${className}`}>
        {children}
    </div>;
};

export default CardHeader;