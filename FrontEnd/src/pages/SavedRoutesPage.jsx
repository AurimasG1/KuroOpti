import { useState, useEffect } from "react";
import { clearRouteHistory, deleteRouteHistory, getDetailedRouteHistory } from "../services/api.js";
import { useNavigate } from "react-router-dom";
import toast from "react-hot-toast";

const SavedRoutesPage = () => {
  const [savedRoutes, setSavedRoutes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [filterText, setFilterText] = useState("");
  const [filterFuel, setFilterFuel] = useState("");
  const [sortBy, setSortBy] = useState("date_desc");
  const [fuelMax, setFuelMax] = useState(150);
  const [maxDistance, setMaxDistance] = useState(400);
  const [dateFilter, setDateFilter] = useState("");
  const [timeRange, setTimeRange] = useState("");

  const navigate = useNavigate();

  const handleReloadRoute = (route) => {
    navigate("/MapPage", {
      state: {
        routeId: route.id,
        startAddress: route.startAddress,
        endAddress: route.endAddress,
        startLat: route.startLat,
        startLng: route.startLng,
        endLat: route.endLat,
        endLng: route.endLng,
        polyline: route.polyline,
        fuelType: route.fuelType,
        distanceKm: route.distanceKm,
        fuelEstimate: route.fuelEstimate,
        stations: route.stations,
        justSaved: true
      },
    });

    toast.success("Maršrutas įkeltas sėkmingai");
  };

  const handleDelete = async (id) => {
    try {
      await deleteRouteHistory(id);
      setSavedRoutes(prev => prev.filter(r => r.id !== id));
      toast.success("Maršrutas ištrintas");
    } catch {
      toast.error("Nepavyko ištrinti maršruto");
    }
  };

  const handleClearAll = async () => {
    try {
      await clearRouteHistory(); // tavo API funkcija
      setSavedRoutes([]);
      toast.success("Istorija išvalyta");
    } catch {
      toast.error("Nepavyko išvalyti istorijos");
    }
  };

  useEffect(() => {
    const fetchHistory = async () => {
      try {
        setLoading(true);
        // Gauname DTO masyvą iš backend
        const data = await getDetailedRouteHistory();

        // DTO struktūra:
        // {
        //   id, routeId, createdAt,
        //   startAddress, endAddress,
        //   startLat, startLng, endLat, endLng,
        //   fuelType, distanceKm, polyline,
        //   fuelEstimate,
        //   stations: [{ id, name, address, prices... }]
        // }
        console.log("Štai ką React gavo iš BackEnd:", data);
        setSavedRoutes(data);
      } catch (err) {
        toast.error("Klaida istorijos puslapyje");
        console.error("Klaida istorijos puslapyje:", err);
        setError("Nepavyko užkrauti maršrutų istorijos iš serverio.");
      } finally {
        setLoading(false);
      }
    };

    fetchHistory();
  }, []);

  // -------------------------------------------------------
  // FILTRAVIMAS
  // -------------------------------------------------------

  const getFuelPrice = (route, fuelType) => {
    return (
      route[fuelType] ??
      route[fuelType.toLowerCase()] ??
      0
    );
  };

  const filteredRoutes = savedRoutes
    .filter((r) => {
      const text = filterText.toLowerCase();
      return (
        (r.startAddress || "").toLowerCase().includes(text) ||
        (r.endAddress || "").toLowerCase().includes(text)
      );
    })
    .filter((r) => {
      if (filterFuel === "") return true;
      return r.fuelType === filterFuel;
    })
    .filter((r) => r.fuelEstimate <= fuelMax)
    .filter((r) => r.distanceKm <= maxDistance)
    .filter((r) => {
      if (!dateFilter) return true;

      const routeDate = new Date(r.createdAt).toISOString().split("T")[0];
      return routeDate === dateFilter;
    }).filter((r) => {
      if (timeRange === "") return true;

      const created = new Date(r.createdAt);
      const now = new Date();

      if (timeRange === "today") {
        return created.toDateString() === now.toDateString();
      }

      if (timeRange === "yesterday") {
        const yesterday = new Date();
        yesterday.setDate(now.getDate() - 1);
        return created.toDateString() === yesterday.toDateString();
      }

      if (timeRange === "week") {
        const weekAgo = new Date();
        weekAgo.setDate(now.getDate() - 7);
        return created >= weekAgo;
      }

      if (timeRange === "month") {
        const monthAgo = new Date();
        monthAgo.setMonth(now.getMonth() - 1);
        return created >= monthAgo;
      }

      return true;
    });

  // -------------------------------------------------------
  // RŪŠIAVIMAS
  // -------------------------------------------------------
  const sortedRoutes = [...filteredRoutes].sort((a, b) => {
    switch (sortBy) {
      case "date_desc":
        return new Date(b.createdAt) - new Date(a.createdAt);
      case "date_asc":
        return new Date(a.createdAt) - new Date(b.createdAt);
      case "distance_desc":
        return b.distanceKm - a.distanceKm;
      case "distance_asc":
        return a.distanceKm - b.distanceKm;
      case "stations_desc":
        return (b.stations?.length || 0) - (a.stations?.length || 0);
      case "stations_asc":
        return (a.stations?.length || 0) - (b.stations?.length || 0);
      default:
        return 0;
    }
  });

  if (loading)
    return (
      <div className="p-6 text-white text-center">Kraunama istorija...</div>
    );

  if (error)
    return (
      <div className="p-6 text-red-500 text-center font-semibold">{error}</div>
    );

  // -------------------------------------------------------
  // UI: PAGRINDINIS TURINYS
  // -------------------------------------------------------
  return (
    <div className="p-6 bg-slate-900 min-h-screen text-white">
      <h1 className="text-2xl font-bold text-lime-500 mb-6">
        Mano išsaugoti maršrutai
      </h1>

      {/* FILTRAI */}
      <div className="flex flex-col md:flex-row gap-4 mb-6">
        <input
          type="text"
          placeholder="Filtruoti pagal adresą..."
          value={filterText}
          onChange={(e) => setFilterText(e.target.value)}
          className="p-2 bg-slate-700 rounded text-white"
        />

        <select
          value={filterFuel}
          onChange={(e) => setFilterFuel(e.target.value)}
          className="p-2 bg-slate-700 rounded text-white"
        >
          <option value="">Visi kuro tipai</option>
          <option value="PetrolPrice">Benzinas</option>
          <option value="DieselPrice">Dyzelinas</option>
          <option value="LpgPrice">Dujos</option>
        </select>

        <div className="flex flex-col">
          <label className="text-white mb-1">Maksimali kuro sąmata</label>
          <input
            type="range"
            min="0"
            max="150"
            value={fuelMax}
            onChange={(e) => setFuelMax(Number(e.target.value))}
            className="w-full"
          />
          <span className="text-white">{fuelMax} €</span>
        </div>

        <div className="flex flex-col">
          <label className="text-white mb-1">Maksimalus atstumas</label>
          <input
            type="range"
            min="0"
            max="300"
            value={maxDistance}
            onChange={(e) => setMaxDistance(Number(e.target.value))}
            className="w-full"
          />
          <span className="text-white">{maxDistance} km</span>
        </div>

        <div className="flex flex-col">
          <label className="text-white mb-1">Filtruoti pagal datą</label>
          <input
            type="date"
            value={dateFilter}
            onChange={(e) => setDateFilter(e.target.value)}
            className="p-2 bg-slate-700 rounded text-white"
          />
        </div>

        <div className="flex flex-col">
          <label className="text-white mb-1">Laiko intervalas</label>
          <select
            value={timeRange}
            onChange={(e) => setTimeRange(e.target.value)}
            className="p-2 bg-slate-700 rounded text-white"
          >
            <option value="">Visi laikai</option>
            <option value="today">Šiandien</option>
            <option value="yesterday">Vakar</option>
            <option value="week">Šią savaitę</option>
            <option value="month">Šį mėnesį</option>
          </select>
        </div>

        <select
          value={sortBy}
          onChange={(e) => setSortBy(e.target.value)}
          className="p-2 bg-slate-700 rounded text-white"
        >
          <option value="date_desc">Naujausi viršuje</option>
          <option value="date_asc">Seniausi viršuje</option>
          <option value="distance_desc">Didžiausias atstumas</option>
          <option value="distance_asc">Mažiausias atstumas</option>
          <option value="stations_desc">Daugiausia stotelių</option>
          <option value="stations_asc">Mažiausiai stotelių</option>
        </select>

        <button
          onClick={handleClearAll}
          className="px-4 py-2 bg-red-600 hover:bg-red-500 rounded text-white font-bold"
        >
          Išvalyti visą istoriją 🗑️
        </button>
      </div>

      {/* JEI NĖRA MARŠRUTŲ */}
      {sortedRoutes.length === 0 ? (
        <p className="text-slate-400 font-medium">
          Pagal pasirinktus filtrus maršrutų nėra.
        </p>
      ) : (
        <div className="space-y-4">
          <p className="text-emerald-400 font-medium">
            Rasta maršrutų: {sortedRoutes.length}
          </p>

          {/* MARŠRUTŲ SĄRAŠAS */}
          <div className="bg-slate-800 p-4 rounded-lg border border-slate-700 shadow-md space-y-4">
            {sortedRoutes.map((route) => (
              <div
                key={route.id}
                className="p-4 bg-slate-700/40 rounded-xl flex flex-col md:flex-row justify-between items-start md:items-center gap-4 border border-slate-600/50"
              >
                {/* KAIRĖ PUSĖ — MARŠRUTO INFORMACIJA */}
                <div>
                  <h3 className="font-bold text-lime-500 text-lg">
                    Maršrutas #{route.id}
                  </h3>

                  <p className="text-sm text-slate-200 font-medium mt-1">
                    {route.startAddress} → {route.endAddress}
                  </p>

                  <p className="text-xs text-slate-400 mt-1">
                    Sukurta:{" "}
                    {new Date(route.createdAt).toLocaleString("lt-LT")}
                  </p>

                  <p className="text-xs text-slate-400 mt-1">
                    Tikras atstumas:{" "}
                    <span className="text-lime-400 font-bold">
                      {route.distanceKm} km
                    </span>
                  </p>

                  <p className="text-xs text-slate-400 mt-1">
                    Kuro tipas:{" "}
                    <span className="text-lime-400 font-bold">
                      {route.fuelType}
                    </span>
                  </p>

                  <p className="text-xs text-slate-400 mt-1">
                    Kuro sąmata:{" "}
                    <span className="text-lime-400 font-bold">
                      {route.fuelEstimate.toFixed(2)} €
                    </span>
                  </p>

                  {/* STOTELIŲ SĄRAŠAS */}
                  {route.stations?.length > 0 && (
                    <div className="mt-2 text-xs text-slate-300">
                      <p className="font-semibold text-slate-200">Stotelės:</p>
                      <ul className="list-disc ml-4">
                        {route.stations.map((s) => (
                          <li key={s.id}>
                            {s.name} — {s.address} ({s.latitude}, {s.longitude})
                            | Benzinas: {s.petrolPrice}€
                            | Dyzelinas: {s.dieselPrice}€
                            | Dujos: {s.lpgPrice}€
                          </li>
                        ))}
                      </ul>
                    </div>
                  )}
                </div>

                {/* DEŠINĖ PUSĖ — MYGTUKAI */}
                <div className="flex gap-3">
                  <button
                    onClick={() => handleReloadRoute(route)}
                    className="px-4 py-2 bg-lime-600 hover:bg-lime-500 text-slate-900 font-bold rounded-lg shadow text-sm"
                  >
                    Įkelti 🔄
                  </button>

                  <button
                    onClick={() => handleDelete(route.id)}
                    className="px-4 py-2 bg-red-500 hover:bg-red-400 text-white font-bold rounded-lg shadow text-sm"
                  >
                    Ištrinti ❌
                  </button>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
};

export default SavedRoutesPage;