import React, { useRef, useState, useEffect, useCallback } from "react";
import { useLocation } from "react-router-dom";
import polyline from "@mapbox/polyline";
import * as turf from "@turf/turf";
import { LuRefreshCcw } from "react-icons/lu";
import toast from "react-hot-toast";
import { getStations, fetchGeocode, saveRouteHistory as sendRouteToBackend, createRouteOnBackend, getDetailedRouteHistory, fetchPricesFromApi } from "../services/api.js";
import MapSection from "../components/map/MapSection.jsx";

const MapPage = () => {
  const location = useLocation();


  const [allStations, setAllStations] = useState([]);
  const [filteredStations, setFilteredStations] = useState([]);
  const [selectedWaypoints, setSelectedWaypoints] = useState([]);
  const [routePoints, setRoutePoints] = useState(null);
  const [startAddr, setStartAddr] = useState("");
  const [endAddr, setEndAddr] = useState("");
  const [isRouteActive, setIsRouteActive] = useState(false);
  const [searchQuery, setSearchQuery] = useState("");
  const [polylineStr, setPolylineStr] = useState("");
  const [routeCoordinates, setRouteCoordinates] = useState([]);
  const [restoredFromHistory, setRestoredFromHistory] = useState(false);
  const [selectedHistoryId, setSelectedHistoryId] = useState("");
  const [autoRestored, setAutoRestored] = useState(false);
  const [loadingPrices, setLoadingPrices] = useState(false);

  const savedUser = JSON.parse(localStorage.getItem("user"));
  const defaultFuel = savedUser?.fuel || "all";
  const [fuelType, setFuelType] = useState(defaultFuel);
  const [distance, setDistance] = useState(15);

  const [wayFrom, setWayFrom] = useState("");
  const [wayTo, setWayTo] = useState("");

  const [history, setHistory] = useState([]);
  const [loading, setLoading] = useState(false);

  const token = localStorage.getItem("accessToken");

  const polylineDecode = (encoded) =>
    polyline.decode(encoded).map(([lat, lng]) => ({ lat, lng }));

  const handleRouteFound = useCallback(
    ({ coordinates, polyline }) => {
      if (polyline) {
        setPolylineStr(polyline);
      }

      if (!coordinates || coordinates.length === 0 || allStations.length === 0)
        return;

      setRouteCoordinates(coordinates);

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

  const restoreRouteFromHistory = useCallback(
    (item) => {
      if (!item) return;
      setRestoredFromHistory(true);

      console.log("[History] Atkuriamas maršrutas:", item);

      setSelectedHistoryId(String(item.id));

      // 1. Formos laukai
      setStartAddr(item.startAddress);
      setEndAddr(item.endAddress);
      setFuelType(item.fuelType);
      setDistance(15);

      // 2. Start/End koordinatės
      setRoutePoints({
        start: [item.startLat, item.startLng],
        end: [item.endLat, item.endLng]
      });

      // 3. Waypoints
      const restoredWaypoints = item.stations.map((s) => ({
        Id: s.id,
        Name: s.name,
        Address: s.address,
        Municipality: s.municipality,
        Latitude: s.latitude,
        Longitude: s.longitude,
        PetrolPrice: s.petrolPrice,
        DieselPrice: s.dieselPrice,
        LpgPrice: s.lpgPrice
      }));

      setSelectedWaypoints(restoredWaypoints);

      // 4. Polyline
      setPolylineStr(item.polyline);

      // 5. Degalinės pagal atstumą
      const decodedCoords = polylineDecode(item.polyline);
      setRouteCoordinates(decodedCoords);

      // handleRouteFound({
      //   coordinates: decodedCoords,
      //   polyline: item.polyline
      // });

      // 6. UI režimas
      setIsRouteActive(true);

      toast.success("Maršrutas atkurtas iš istorijos");
    },
    [handleRouteFound]
  );

  useEffect(() => {
    if (autoRestored) return;
    if (!location.state?.routeId) return;
    if (history.length === 0) return;

    const item = history.find(h => h.id === location.state.routeId);
    if (item) {
      restoreRouteFromHistory(item);
      setAutoRestored(true);
    }
  }, [history, autoRestored, location.state]);

  const loadHistory = (e) => {
    const id = Number(e.target.value);
    setSelectedHistoryId(id);

    if (!id) return;

    const item = history.find(h => h.id === id);
    if (item) restoreRouteFromHistory(item);
  };


  useEffect(() => {
    const load = async () => {
      try {
        const h = await getDetailedRouteHistory(token);
        setHistory(Array.isArray(h) ? h : []);
      } catch (err) {
        console.error("Nepavyko gauti istorijos:", err);
        setHistory([]);
      }
    };

    load();
  }, []);

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

  useEffect(() => {
    if (routeCoordinates.length > 0) {
      handleRouteFound({
        coordinates: routeCoordinates,
        polyline: polylineStr
      });
    }
  }, [distance, routeCoordinates, polylineStr, restoredFromHistory]);


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

    const filtered = filteredStations.filter((s) => {
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


    if (fuelType !== "all") {
      return [...filtered].sort((a, b) => {
        const priceA = parseFloat(a[fuelType]) || 0;
        const priceB = parseFloat(b[fuelType]) || 0;
        return priceA - priceB;
      });
    }

    return filtered;
  }, [filteredStations, searchQuery, fuelType]);

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

  const handleRouteSearch = async (e) => {
    e.preventDefault();
    if (!startAddr || !endAddr) return;

    setRestoredFromHistory(false);

    setLoading(true);
    setIsRouteActive(true);

    setSelectedWaypoints([]);
    setFilteredStations([]);
    setPolylineStr(null);

    try {
      const [start, end] = await Promise.all([
        fetchGeocode(startAddr),
        fetchGeocode(endAddr)
      ]);

      if (start && end) {
        setRoutePoints({
          start: [start[0], start[1]],
          end: [end[0], end[1]]
        });
      } else {
        toast.error("Adresas nerastas")
      }
    } catch (error) {
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  const handleStartRoute = async () => {
    if (!routePoints?.start || !routePoints?.end) {
      toast.error("Pirmiausia įveskite maršrutą!")
      return;
    }

    if (!polylineStr) {
      toast.error("Maršrutas dar nesugeneruotas - palaukite sekundę");
      return;
    }

    const currentToken = localStorage.getItem("accessToken");

    try {
      const createdRoute = await createRouteOnBackend({
        startLat: routePoints.start[0],
        startLng: routePoints.start[1],
        endLat: routePoints.end[0],
        endLng: routePoints.end[1],
        polyline: polylineStr
      }, currentToken);

      const tikriStoteliuId = selectedWaypoints
        .map((wp) => wp.Id || wp.id)
        .filter((id) => id !== undefined && !String(id).startsWith("hist-"));

      await sendRouteToBackend({
        routeId: createdRoute.id,
        startAddress: startAddr,
        endAddress: endAddr,
        startLat: routePoints.start[0],
        startLng: routePoints.start[1],
        endLat: routePoints.end[0],
        endLng: routePoints.end[1],
        fuelType,
        polyline: polylineStr,
        stations: selectedWaypoints.map(s => ({
          id: s.Id || s.id,
          name: s.Name || s.name,
          municipality: s.Municipality || s.municipality || "",
          address: s.Address || s.address || "",
          latitude: s.Latitude || s.latitude,
          longitude: s.Longitude || s.longitude,
          petrolPrice: s.PetrolPrice ?? s.petrolPrice ?? 0,
          dieselPrice: s.DieselPrice ?? s.dieselPrice ?? 0,
          lpgPrice: s.LpgPrice ?? s.lpgPrice ?? 0
        })),
      }, currentToken);

      console.log("Istorija išsaugota sėkmingai!");
      toast.success("Maršrutas sėkmingai išsaugotas");
    } catch (dbErr) {
      toast.error("Nepavyko išsaugoti istorijos");
      console.error("Nepavyko išsaugoti istorijos:", dbErr);
    }

    const origin = `${routePoints.start[0]},${routePoints.start[1]}`;
    const destination = `${routePoints.end[0]},${routePoints.end[1]}`;
    const waypointString = selectedWaypoints
      .map((wp) => `${wp.Latitude},${wp.Longitude}`)
      .join("|");

    // -----------------------------
    // PILNAS STATE RESET (kaip refresh)
    // -----------------------------
    setRestoredFromHistory(false);
    setIsRouteActive(false);

    setSelectedWaypoints([]);
    setFilteredStations([]);

    setRoutePoints(null);
    setRouteCoordinates([]);

    setPolylineStr("");

    setStartAddr("");
    setEndAddr("");

    setDistance(15);
    setSearchQuery("");
    setWayFrom("");
    setWayTo("");
    setSelectedHistoryId("");

    const googleMapsUrl = `https://www.google.com/maps/dir/?api=1&origin=${origin}&destination=${destination}${waypointString ? `&waypoints=${waypointString}` : ""}&travelmode=driving`;
    window.open(googleMapsUrl, "_blank");
    const h = await getDetailedRouteHistory(token);
    setHistory(h);
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
          PetrolPrice: station.PetrolPrice,
          DieselPrice: station.DieselPrice,
          LpgPrice: station.LpgPrice
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

  const handleUpdatePrices = async () => {
    setLoadingPrices(true);
    try {
      toast.loading("Atnaujinama...");
      const result = await fetchPricesFromApi();
      toast.dismiss();
      toast.success("Kainos sėkmingai atnaujintos!");
      console.log("Atnaujinimo rezultatas:", result);
    } catch (err) {
      toast.dismiss();
      toast.error("Nepavyko atnaujinti kainų");
      console.error(err);
    }
    setLoadingPrices(false);
  }

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

            <div className="flex flex-col lg:flex-row lg:items-center lg:justify-between mt-2 gap-3">

              {/* Title */}
              <h2 className="text-xl font-bold text-lime-500 flex-shrink-0">
                Kelionės planas
              </h2>

              {/* Select */}
              <select
                value={selectedHistoryId}
                onChange={loadHistory}
                className="bg-slate-700 p-2 rounded w-full lg:max-w-[55%] xl:max-w-[60%]"
              >
                <option value="">Pasirink maršrutą</option>
                {history.map(h => (
                  <option key={h.id} value={h.id}>
                    {h.startAddress} → {h.endAddress} ({h.distanceKm} km)
                  </option>
                ))}
              </select>

              {/* Refresh button */}
              <div className="group relative flex items-center justify-end lg:justify-center cursor-pointer flex-shrink-0">
                <span className="absolute -top-3 -left-9 opacity-0 group-hover:opacity-100 transition-opacity duration-300 text-sm text-gray-200 text-nowrap">
                  Atnaujinti kainas
                </span>

                <LuRefreshCcw
                  onClick={handleUpdatePrices}
                  className={`bg-lime-600 p-2 rounded-lg text-3xl lg:text-4xl text-white transition-colors group-hover:bg-lime-500 ${loadingPrices ? "animate-spin" : ""
                    }`}
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
            polyline={polylineStr}
          />
        </div>
      </main>
    </div>
  );
};

export default MapPage;