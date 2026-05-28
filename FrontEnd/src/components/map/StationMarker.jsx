import React from "react";
import { Marker, Popup } from "react-leaflet";



const StationMarker = ({ station, addedWaypoints = [], onToggleRoute }) => {  
  const lat = parseFloat(station.Latitude || station.latitude || station.lat);
  const lng = parseFloat(station.Longitude || station.longitude || station.lng);
 
  if (isNaN(lat) || isNaN(lng)) {
    return null;
  }

  const isInRoute = addedWaypoints.some(
    (wp) => (wp.Id || wp.id) === (station.Id || station.id)
  );

  return (
    <Marker
      position={[lat, lng]} 
      eventHandlers={{
        add: (e) => {
          requestAnimationFrame(() => {
            if (e.target && e.target._icon) {
              e.target._icon.style.filter = "hue-rotate(-120deg)";
            }
          });
        },
      }}
    >
      <Popup>
        <div className="text-slate-900 min-w-35">
          <strong className="block border-b mb-1">{station.Name}</strong>
          <div className="text-[10px] text-slate-500 mb-2">
            {station.Address}, {station.Municipality}
          </div>
          
          <div className="space-y-1 text-xs">
            <div className="flex justify-between">
              <span>Benzinas:</span> <span className="font-bold">{station.PetrolPrice} €</span>
            </div>
            <div className="flex justify-between">
              <span>Dyzelinas:</span> <span className="font-bold">{station.DieselPrice} €</span>
            </div>
            <div className="flex justify-between">
              <span>Dujos:</span> <span className="font-bold">{station.LpgPrice} €</span>
            </div>
          </div>
 
          <button
            type="button"
            onClick={() => onToggleRoute && onToggleRoute(station)}
            className={`mt-3 w-full text-[10px] p-2 rounded transition-colors uppercase font-bold text-white ${
              isInRoute ? 'bg-red-600 hover:bg-red-500' : 'bg-lime-600 hover:bg-lime-500'
            }`}
          >
            {isInRoute ? 'Nuimti iš maršruto' : 'Pridėti į maršrutą'}
          </button>
        </div>
      </Popup>
    </Marker>
  );
};

export default StationMarker;