import React, { useState, useEffect, useCallback } from "react";
import MapSection from "../components/map/MapSection.jsx";
import * as turf from "@turf/turf";
import { LuRefreshCcw } from "react-icons/lu";
import { getStations, fetchGeocode, saveRouteHistory as sendRouteToBackend } from "../services/api.js";
import { useLocation } from "react-router-dom";

const MapPage = () => {
  const [allStations, setAllStations] = useState([]);
  const [filteredStations, setFilteredStations] = useState([]);
  const [selectedWaypoints, setSelectedWaypoints] = useState([]);
  const [loading, setLoading] = useState(false);
  const [routePoints, setRoutePoints] = useState(null);
  const [startAddr, setStartAddr] = useState("");
  const [endAddr, setEndAddr] = useState("");
  const [isRouteActive, setIsRouteActive] = useState(false);
  const [searchQuery, setSearchQuery] = useState("");

  const savedUser = JSON.parse(localStorage.getItem("user"));
  const defaultFuel = savedUser?.fuel || "all";
  const [fuelType, setFuelType] = useState(defaultFuel);
  const [distance, setDistance] = useState(15);

  const [wayFrom, setWayFrom] = useState("");
  const [wayTo, setWayTo] = useState("");

  const token = localStorage.getItem("accessToken");

  const location = useLocation();

  useEffect(() => {
    if (
      location.state &&
      location.state.reloadedStations &&
      location.state.reloadedStart !== undefined &&
      location.state.reloadedEnd !== undefined
    ) {
      console.log("[KuroOpti] Gauti duomenys iš istorijos:", location.state);

      // Naudojame atstatymo funkciją
      handleReloadFromHistory(
        location.state.reloadedStations,
        location.state.reloadedStart,
        location.state.reloadedEnd,
      );
    }
  }, [location.state, allStations]); 

  const handleReloadFromHistory = async (stations, startText, endText) => {
    if (!stations || stations.length === 0) return;
    
    setLoading(true);
    setIsRouteActive(true);

    try {
      console.log("[KuroOpti] Atstatomi stotelių ID iš istorijos:", stations);

      const surastosDegalines = stations.map(id => {
        return allStations.find(s => String(s.Id || s.id) === String(id));
      }).filter(Boolean);

      const paruostosStoteles = surastosDegalines.map((st) => ({
        Id: st.Id || st.id,
        id: st.Id || st.id,
        Name: st.Name || st.name,
        name: st.Name || st.name,
        Latitude: Number(st.Latitude || st.latitude),
        latitude: Number(st.Latitude || st.latitude),
        Longitude: Number(st.Longitude || st.longitude),
        longitude: Number(st.Longitude || st.longitude)
      }));

      setSelectedWaypoints(paruostosStoteles);

      let finalStartText = startText;
      let finalEndText = endText;

      if (!finalStartText && surastosDegalines.length > 0) {
        finalStartText = surastosDegalines[0].Address || surastosDegalines[0].Name;
      }
      if (!finalEndText && surastosDegalines.length > 0) {
        finalEndText = surastosDegalines[surastosDegalines.length - 1].Address || surastosDegalines[surastosDegalines.length - 1].Name;
      }

      setStartAddr(finalStartText || "Pradžia");
      setEndAddr(finalEndText || "Pabaiga");

      const startCoord = await fetchGeocode(finalStartText);
      const endCoord = await fetchGeocode(finalEndText);

      if (startCoord && endCoord) {
        setRoutePoints({ start: startCoord, end: endCoord });
        console.log("[KuroOpti] Maršrutas sėkmingai atkurtas aplink išsaugotas degalines!");
      } else {
        console.warn("[KuroOpti] Nepavyko gauti koordinačių pagal degalinių adresus.");
      }

    } catch (err) {
      console.error("Klaida handleReloadFromHistory:", err);
    } finally {
      setLoading(false);
    }
  };

  const handleSliderChange = (e) => {
    setDistance(e.target.value);
  };

  const getFormattedDate = () => {
    return new Date().toLocaleString("lt-LT", {
      month: "long",
      day: "2-digit",
    });
  };
  const [updatedDate, setUpdatedDate] = useState(getFormattedDate);

  const updateDateOnClick = () => {
    setUpdatedDate(getFormattedDate());
  };

  const displayStations = React.useMemo(() => {
    return filteredStations.filter((s) => {
      const simplify = (text) =>
        text
          ?.toString()
          .toLowerCase()
          .normalize("NFD")
          .replace(/[\u0300-\u036f]/g, "") || "";

      const query = searchQuery.toLowerCase();
      const normalizedQuery = simplify(query);
      const matchesText =
        simplify(s.Name).includes(normalizedQuery) ||
        simplify(s.Address).includes(normalizedQuery) ||
        simplify(s.Municipality).includes(normalizedQuery);

      let matchesFuel = true;
      if (fuelType !== "all") {
        const kaina = parseFloat(s[fuelType]);
        matchesFuel = kaina > 0;
      }

      return matchesText && matchesFuel;
    });
  }, [filteredStations, searchQuery, fuelType]);

  useEffect(() => {
    const loadStations = async () => {
      try {
        const data = await getStations();
        setAllStations(data);
        setFilteredStations(data);
      } catch (error) {
        console.error("Nepavyko užkrauti degalinių iš BackEnd:", error);
      }
    };

    loadStations();
  }, []);

  const handleAddToRoute = (station) => {
    setSelectedWaypoints((prev) => {
      if (prev.find((p) => p.id === station.id)) return prev;

      const newWaypoint = {
        Id: station.Id,
        Latitude: station.Latitude,
        Longitude: station.Longitude,
        Name: station.Name,
      };

      const updated = [...prev, newWaypoint];

      if (routePoints && routePoints.start) {
        updated.sort((a, b) => {
          const distA = Math.sqrt(
            Math.pow(a.Latitude - routePoints.start[0], 2) +
              Math.pow(a.Longitude - routePoints.start[1], 2),
          );
          const distB = Math.sqrt(
            Math.pow(b.Latitude - routePoints.start[0], 2) +
              Math.pow(b.Longitude - routePoints.start[1], 2),
          );
          return distA - distB;
        });
      }
      return updated;
    });
  };

  const handleRemoveWaypoint = (id) => {
    setSelectedWaypoints((prev) => prev.filter((p) => p.Id !== id));
  };

  const handleRouteFound = useCallback(
    (coordinates) => {
      if (!coordinates || coordinates.length === 0 || allStations.length === 0)
        return;

      try {
        const turfCoords = coordinates.map((c) => [
          c.lng || c.Longitude,
          c.lat || c.Latitude,
        ]);

        const line = turf.lineString(turfCoords);
        const buffer = turf.buffer(line, distance, { units: "kilometers" });

        const found = allStations.filter((station) => {
          const sLng = Number(
            station.Longitude || station.longitude || station.lng,
          );
          const sLat = Number(
            station.Latitude || station.latitude || station.lat,
          );

          if (isNaN(sLng) || isNaN(sLat)) return false;

          const stationPt = turf.point([sLng, sLat]);
          return turf.booleanPointInPolygon(stationPt, buffer);
        });

        setFilteredStations(found);
      } catch (err) {
        console.error("Filtravimo klaida:", err);
      }
    },
    [allStations, distance],
  );

  const handleRouteSearch = async (e) => {
    e.preventDefault();
    if (!startAddr || !endAddr) return;
    setLoading(true);
    setIsRouteActive(true);
    setSelectedWaypoints([]);
    setFilteredStations([]);

    try {
      const [start, end] = await Promise.all([
        fetchGeocode(startAddr),
        fetchGeocode(endAddr)
      ]);

      if (start && end) {
        setRoutePoints({ start, end });
      } else {
        alert("Adresas nerastas.");
      }
    } catch (error) {
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  const handleStartRoute = async () => {
    if (!routePoints?.start || !routePoints?.end) {
      alert("Pirmiausia įveskite maršrutą!");
      return;
    }

    try {
      const currentToken = localStorage.getItem("accessToken");
      const tikriStoteliuId = selectedWaypoints
        .map((wp) => wp.Id || wp.id)
        .filter((id) => id !== undefined && !String(id).startsWith("hist-"));

      await sendRouteToBackend({ 
        routeId: 0, 
        startAddress: startAddr,
        endAddress: endAddr,    
        selectedStationIds: tikriStoteliuId 
      }, currentToken);

      console.log("Istorija išsaugota sėkmingai!");
    } catch (dbErr) {
      console.error("Nepavyko išsaugoti istorijos:", dbErr);
    }

    const origin = `${routePoints.start[0]},${routePoints.start[1]}`;
    const destination = `${routePoints.end[0]},${routePoints.end[1]}`;
    const waypointString = selectedWaypoints
      .map((wp) => `${wp.Latitude},${wp.Longitude}`)
      .join("|");
      
    const googleMapsUrl = `https://www.google.com/maps/dir/?api=1&origin=${origin}&destination=${destination}${waypointString ? `&waypoints=${waypointString}` : ""}&travelmode=driving`;
    window.open(googleMapsUrl, "_blank");
  };

  const handleToggleRoute = (station) => {
    setSelectedWaypoints((prev) => {
      const exists = prev.some(
        (p) => (p.Id || p.id) === (station.Id || station.id),
      );

      if (exists) {
        return prev.filter((p) => p.Id !== station.Id && p.Id !== station.id);
      } else {
        const newWaypoint = {
          Id: station.Id || station.id,
          Latitude: station.Latitude,
          Longitude: station.Longitude,
          Name: station.Name,
        };

        const updated = [...prev, newWaypoint];
        if (routePoints && routePoints.start) {
          updated.sort((a, b) => {
            const distA = Math.sqrt(
              Math.pow(a.Latitude - routePoints.start[0], 2) +
                Math.pow(a.Longitude - routePoints.start[1], 2),
            );
            const distB = Math.sqrt(
              Math.pow(b.Latitude - routePoints.start[0], 2) +
                Math.pow(b.Longitude - routePoints.start[1], 2),
            );
            return distA - distB;
          });
        }
        return updated;
      }
    });
  };

  return (
    <div className="flex flex-col h-screen bg-slate-900 text-white sm:flex-row">
      <main className="flex flex-col-reverse md:flex-row flex-1 p-4 gap-4 overflow-hidden">
        <div className="w-full md:w-1/4 bg-slate-800 p-4 rounded-xl shadow-xl flex flex-col overflow-y-auto max-h-[40vh] md:max-h-full">
          <div className="flex flex-col gap-2 mb-4">
            {/* 1. Filters */}
            <div className="flex flex-col gap-2 bg-slate-700/50 p-2 rounded-lg">
              <label className="text-xs text-slate-400 uppercase font-bold">
                Kuro tipas:
              </label>
              <select
                className="p-2 bg-slate-700 rounded text-sm outline-none border border-slate-600 focus:border-lime-500"
                onChange={(e) => setFuelType(e.target.value)}
                value={fuelType}
              >
                <option value="all">Visi degalai</option>
                <option value="PetrolPrice">Benzinas</option>
                <option value="DieselPrice">Dyzelinas</option>
                <option value="LpgPrice">Dujos</option>
              </select>

              <label className="text-xs text-slate-400 uppercase font-bold mt-2">
                Degalinės paieška:
              </label>
              <input
                type="text"
                placeholder="Ieškoti pagal pavadinimą..."
                className="p-2 bg-slate-700 rounded text-sm outline-none border border-slate-600 focus:border-lime-500"
                onChange={(e) => setSearchQuery(e.target.value)}
              />
              <div>
                <label className="text-xs text-slate-400 uppercase font-bold mt-2">
                  Degalinių atstumas nuo maršruto:
                </label>
                <div className="p-2 bg-slate-700 rounded text-sm outline-none border border-slate-600">
                  <input
                    type="range"
                    min="1"
                    max="30"
                    value={distance}
                    onChange={handleSliderChange}
                    className="w-full accent-lime-600"
                  />
                  <div className="text-xs text-slate-400 uppercase font-bold my-2">
                    <span>{distance} km</span>
                  </div>
                </div>
              </div>
            </div>

            <div className="flex flex-row items-center justify-between mt-2">
              <h2 className="text-xl font-bold text-lime-500">
                Kelionės planas
              </h2>
              <div className="group relative flex items-center cursor-pointer">
                <span className="absolute -top-3 -left-9 opacity-0 group-hover:opacity-100 transition-opacity duration-300 text-sm text-gray-200 text-nowrap">
                  Atnaujinti kainas
                </span>

                <LuRefreshCcw
                  onClick={updateDateOnClick}
                  className="bg-lime-600 m-2 p-2 rounded-lg text-4xl text-white transition-colors group-hover:bg-lime-500"
                />
              </div>
            </div>
          </div>

          <form onSubmit={handleRouteSearch} className="space-y-3 mb-6">
            <input
              type="text"
              value={startAddr}
              onChange={(e) => setStartAddr(e.target.value)}
              placeholder="Iš kur"
              className="w-full p-2 rounded bg-slate-700 border border-slate-600"
            />
            <input
              type="text"
              value={endAddr}
              onChange={(e) => setEndAddr(e.target.value)}
              placeholder="Į kur"
              className="w-full p-2 rounded bg-slate-700 border border-slate-600"
            />
            <button
              type="submit"
              disabled={loading}
              className="w-full bg-lime-600 p-2 rounded font-bold hover:bg-lime-500"
            >
              {loading ? "Skaičiuojama..." : "Rodyti maršrutą"}
            </button>
          </form>

          {/* Waypoint stops */}
          {selectedWaypoints.length > 0 && (
            <div className="mb-6 p-3 bg-indigo-900/30 border border-indigo-500/50 rounded-lg">
              <h3 className="text-xs font-bold text-indigo-400 uppercase mb-2">
                Sustojimai ({selectedWaypoints.length})
              </h3>
              {selectedWaypoints.map((wp, idx) => (
                <div
                  key={wp.Id || wp.id || idx}
                  className="flex justify-between items-center text-xs mb-1 bg-slate-700 p-2 rounded"
                >
                  <span>{wp.Name || wp.name || wp.address || "Degalinė"}</span>

                  <button
                    onClick={() => handleRemoveWaypoint(wp.Id || wp.id)}
                    className="text-red-400 font-bold px-1 hover:text-red-300 transition-colors"
                  >
                    ✕
                  </button>
                </div>
              ))}
            </div>
          )}

          {/* 2. List */}
          <div className="flex-1">
            <div className="h-160 overflow-y-scroll">
              <h3 className="text-sm font-semibold mb-2 text-slate-400 italic">
                Degalinės pakeliui ({displayStations.length})
              </h3>

              {displayStations.map((s) => (
                <div
                  key={s.Id}
                  className="p-3 mb-2 bg-slate-700 rounded-lg border border-slate-600 hover:border-lime-500/50 transition-colors "
                >
                  <p className="font-bold text-lime-400 text-xl">{s.Name}</p>
                  <p className="text-[10px] text-slate-400">{s.Address}</p>

                  <div className="flex flex-col gap-2 py-2">
                    <div className="flex flex-row justify-between items-center">
                      <p className="font-bold text-slate-400">Benzinas:</p>
                      <p className="font-bold text-lime-400 text-sm">
                        {s.PetrolPrice} €
                      </p>
                    </div>
                    <div className="flex flex-row justify-between items-center">
                      <p className="font-bold text-slate-400">Dyzelinas:</p>
                      <p className="font-bold text-lime-400 text-sm">
                        {s.DieselPrice} €
                      </p>
                    </div>
                    <div className="flex flex-row justify-between items-center">
                      <p className="font-bold text-slate-400">Dujos:</p>
                      <p className="font-bold text-lime-400 text-sm">
                        {s.LpgPrice} €
                      </p>
                    </div>
                  </div>

                  <button
                    type="button"
                    onClick={() => handleAddToRoute(s)}
                    className="mt-2 w-full text-[10px] bg-slate-600 hover:bg-lime-600 p-2 rounded transition-colors font-bold"
                  >
                    Pridėti į maršrutą
                  </button>
                </div>
              ))}
            </div>

            {displayStations.length > 0 && (
              <button
                onClick={handleStartRoute}
                className="w-full bg-lime-600 text-white p-3 rounded font-bold hover:bg-lime-500 mt-4 shadow-lg"
              >
                Pridėti maršrutą ir išsaugoti
              </button>
            )}
          </div>
          <div className="flex flex-row items-center justify-center gap-2 text-nowrap">
            <p>Atnaujinta: </p>
            <div>{updatedDate}</div>
          </div>

          <div className="flex flex-row items-center justify-center gap-2">
            <p>Šaltinis: </p>
            <a
              href="https://www.ena.lt/degalu-kainos-degalinese/"
              target="_blank"
              rel="noreferrer"
            >
              LEA
            </a>
          </div>
        </div>

        {/* 3. Map  */}
        <div className="flex-1 relative rounded-xl overflow-hidden border border-slate-700 bg-slate-800">
          <MapSection
            stations={displayStations}
            routePoints={routePoints}
            addedWaypoints={selectedWaypoints}
            onRouteFound={handleRouteFound}
            onAddToRoute={handleAddToRoute}
            onToggleRoute={handleToggleRoute}
          />
        </div>
      </main>
    </div>
  );
};

export default MapPage;