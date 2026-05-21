import React from "react";
import { Marker, Popup } from "react-leaflet";

const StationMarker = ({ station, onAddToRoute }) => { 
  return (
    <Marker
      position={[station.lat, station.lng]}
       
      eventHandlers={{
        add: (e) => {
          if (e.target._icon) {
            e.target._icon.style.filter = "hue-rotate(-120deg)";
          }
        },
      }}
    >
      <Popup>
        <div className="text-slate-900 min-w-35">
          <strong className="block border-b mb-1">{station.brand}</strong>
          <div className="text-[10px] text-slate-500 mb-2">
            {station.address}, {station.city}
          </div>
          
          <div className="space-y-1 text-xs">
            <div className="flex justify-between">
              <span>Benzinas:</span> <span className="font-bold">{station.gasoline} €</span>
            </div>
            <div className="flex justify-between">
              <span>Dyzelinas:</span> <span className="font-bold">{station.diesel} €</span>
            </div>
            <div className="flex justify-between">
              <span>Dujos:</span> <span className="font-bold">{station.gas} €</span>
            </div>
          </div>

          <button
            type="button"
            onClick={() => onAddToRoute && onAddToRoute(station)}
            className="mt-3 w-full text-[10px] bg-lime-600 hover:bg-lime-500 text-white p-2 rounded transition-colors uppercase font-bold"
          >
            Pridėti į maršrutą
          </button>
        </div>
      </Popup>
    </Marker>
  );
};

export default StationMarker;