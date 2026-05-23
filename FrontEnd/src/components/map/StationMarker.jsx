import React from "react";
import { Marker, Popup } from "react-leaflet";

const StationMarker = ({ station, onAddToRoute }) => { 
  // Saugiai ištraukiame platumą ir ilgumą iš bet kokio varianto
  const lat = parseFloat(station.Latitude || station.latitude || station.lat);
  const lng = parseFloat(station.Longitude || station.longitude || station.lng);

  // Jei koordinatės sugadintos, markerio išvis nepiešiame, kad negadintų žemėlapio
  if (isNaN(lat) || isNaN(lng)) {
    return null;
  }

  return (
    <Marker
      position={[lat, lng]} // Naudojame saugiai ištrauktus skaičius
      eventHandlers={{
        add: (e) => {
          // Naudojame requestAnimationFrame, kad išvengtume lenktynių sąlygų (race conditions)
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