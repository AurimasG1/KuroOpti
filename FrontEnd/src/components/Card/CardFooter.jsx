import React from "react";

const CardFooter = ({children, className = ""}) =>{
    return <div className={`mt-4 pt-4 border-t border-gray-200 ${className}`}>
        {children}
    </div>
}

export default CardFooter;