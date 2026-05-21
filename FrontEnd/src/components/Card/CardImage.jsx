import React from "react";

const CardImage = ({ src, alt = "Card Image", className = "" }) => {
  return (
    <div className={`w-24 h-24 overflow-hidden rounded-full`}>
      {src ? (
        <img src={src} className="w-full h-full object-cover" />
      ) : (
        <div
          className="w-full h-full bg-linear-to-r from-gray-200 to-gray-400 flex
             items-center justify-center"
        >
          <span className="text-gray-400 text-sm">No image avalable</span>
        </div>
      )}
    </div>
  );
};

export default CardImage;
