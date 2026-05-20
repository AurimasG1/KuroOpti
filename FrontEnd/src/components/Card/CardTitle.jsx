import React from "react";

const CardTitle = ({children, className = ""}) =>{
    return <h3 className={`text-2xl font-bold mb-2 whitespace-pre-line`}>
        {children}
    </h3>
}

export default CardTitle;