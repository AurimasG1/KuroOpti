import { useState, useEffect } from "react";
import { getDetailedRouteHistory } from "../services/api.js";
import { useNavigate } from "react-router-dom";

const SavedRoutesPage = () => {
  const [savedRoutes, setSavedRoutes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const navigate = useNavigate();

  const handleReloadRoute = (route) => {
    console.log("Maršruto duomenys iš DB:", route);

    if (!route.stations || route.stations.length === 0) {
      // alert("Šis maršrutas neturi išsaugotų stotelių.");
      console.log("Maršrutas be stotelių — kraunam tik start/end.");
      // return;
    }

    const start = route.startAddress || route.StartAddress || "";
    const end = route.endAddress || route.EndAddress || "";

    navigate("/MapPage", {
      state: {
        reloadedStations: route.stations,
        startLat: route.startLat,
        startLng: route.startLng,
        endLat: route.endLat,
        endLng: route.endLng
      },
    });
  };

  useEffect(() => {
    const token = localStorage.getItem("accessToken");
    if (!token) return;
    console.log("API siunčia token:", token);


    const fetchHistory = async () => {
      try {
        setLoading(true);
        const data = await getDetailedRouteHistory();
        console.log("Štai ką React gavo iš BackEnd:", data);
        setSavedRoutes(data);
      } catch (err) {
        console.error("Klaida istorijos puslapyje:", err);
        setError("Nepavyko užkrauti maršrutų istorijos iš serverio.");
      } finally {
        setLoading(false);
      }
    };

    fetchHistory();
  }, []);

  if (loading)
    return (
      <div className="p-6 text-white text-center">Kraunama istorija...</div>
    );
  if (error)
    return (
      <div className="p-6 text-red-500 text-center font-semibold">{error}</div>
    );

  return (
    <div className="p-6 bg-slate-900 min-h-screen text-white">
      <h1 className="text-2xl font-bold text-lime-500 mb-6">
        Mano išsaugoti maršrutai
      </h1>

      {savedRoutes.length === 0 ? (
        <p className="text-slate-400 font-medium">
          Kol kas neturite išsaugotų maršrutų.
        </p>
      ) : (
        <div className="space-y-4">
          <p className="text-emerald-400 font-medium">
            Sėkmingai rasta maršrutų: {savedRoutes.length}
          </p>
          <div className="bg-slate-800 p-4 rounded-lg border border-slate-700 shadow-md space-y-4">
            {savedRoutes.map((route, idx) => {
              const hasAddress = route.startAddress || route.StartAddress;
              const marsrutoTekstas = hasAddress
                ? `${route.startAddress || route.StartAddress} ➔ ${route.endAddress || route.EndAddress}`
                : `Maršrutas per degalinę (ID: ${route.selectedStations?.join(", ") || "Nenurodyta"})`;

              return (
                <div key={route.id || idx} className="p-4 bg-slate-700/40 rounded-xl flex flex-col md:flex-row justify-between items-start md:items-center gap-4 border border-slate-600/50">
                  <div>
                    <h3 className="font-bold text-lime-500 text-lg">
                      Maršrutas #{route.id}
                    </h3>

                    <p className="text-sm text-slate-200 font-medium mt-1">
                      {marsrutoTekstas}
                    </p>

                    <p className="text-xs text-slate-400 mt-1">
                      Išsaugojimo data:{" "}
                      {route.plannedAt
                        ? new Date(route.plannedAt).toLocaleString("lt-LT")
                        : "Nenurodyta"}
                    </p>
                    <div className="text-xs text-slate-400 mt-2">
                      Sustojimų skaičius:{" "}
                      <span className="text-lime-400 font-bold">
                        {route.selectedStations?.length || 0}
                      </span>
                    </div>
                  </div>

                  <button
                    onClick={() => handleReloadRoute(route)}
                    className="px-4 py-2 bg-lime-600 hover:bg-lime-500 text-slate-900 font-bold rounded-lg shadow transition-colors text-sm self-start md:self-center"
                  >
                    Įkelti maršrutą iš naujo 🔄
                  </button>
                </div>
              );
            })}
          </div>
        </div>
      )}
    </div>
  );
};

export default SavedRoutesPage;