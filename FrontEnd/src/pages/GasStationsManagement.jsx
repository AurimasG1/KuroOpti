import React, { useState, useEffect, useRef } from "react";
import { toast } from "react-hot-toast";

export default function GasStationsManagement() {
  const [gasStations, setGasStations] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const initialFormState = {
    name: "",
    municipality: "",
    address: "",
    latitude: 54.687,
    longitude: 25.279,
  };

  const [formData, setFormData] = useState(initialFormState);
  const [editingGasStationId, setEditingGasStationId] = useState(null);
  const [isFormOpen, setIsFormOpen] = useState(false);

  const [currentPrices, setCurrentPrices] = useState({
    diesel: 0,
    petrol: 0,
    lpg: 0,
  });

  const baseUrl = (
    import.meta.env.VITE_API_URL || "http://localhost:5211"
  ).trim();

  const cleanBaseUrl = baseUrl.endsWith("/") ? baseUrl.slice(0, -1) : baseUrl;
  const apiUrl = `${cleanBaseUrl}/api/FuelStation`;

  console.log("Mano siunčiamas užklausos adresas:", apiUrl);

  const formRef = useRef(null);

  const scrollToForm = () => {
    setTimeout(() => {
      formRef.current?.scrollIntoView({ behavior: "smooth", block: "start" });
    }, 100);
  };

  const fetchGasStations = async () => {
    try {
      setLoading(true);
      const response = await fetch(apiUrl);
      if (!response.ok) throw new Error("Nepavyko užkrauti degalinių");
      const data = await response.json();
      setGasStations(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchGasStations();
  }, []);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    const isCoordinate = ["latitude", "longitude"].includes(name);
    setFormData({
      ...formData,
      [name]: isCoordinate ? parseFloat(value) || 0 : value,
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const method = editingGasStationId ? "PUT" : "POST";
      const url = editingGasStationId
        ? `${apiUrl}/${editingGasStationId}`
        : apiUrl;

      const payload = {
        id: editingGasStationId || 0,
        name: formData.name,
        municipality: formData.municipality,
        address: formData.address,
        latitude: formData.latitude,
        longitude: formData.longitude,
        dieselPrice: currentPrices.diesel,
        petrolPrice: currentPrices.petrol,
        lpgPrice: currentPrices.lpg,
      };

      const response = await fetch(url, {
        method: method,
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });

      if (!response.ok) throw new Error("Nepavyko išsaugoti degalinės");

      if (editingGasStationId) {
        toast.success("Degalinės informacija atnaujinta sėkmingai!");
      } else {
        toast.success("Degalinė pridėta sėkmingai!");
      }
      handleFormClose();
      fetchGasStations();
    } catch (err) {
      toast.error("Klaida: " + err.message);
    }
  };

  const handleDelete = async (id, name) => {
    if (!window.confirm(`Ar tikrai norite ištrinti degalinę "${name}"?`))
      return;
    try {
      const response = await fetch(`${apiUrl}/${id}`, { method: "DELETE" });
      if (!response.ok) throw new Error("Nepavyko ištrinti degalinės");
      toast.success("Degalinė ištrinta sėkmingai!");
      fetchGasStations();
    } catch (err) {
      toast.error("Klaida: " + err.message);
    }
  };

  const handleEdit = (gasStation) => {
    setEditingGasStationId(gasStation.id);
    setFormData({
      name: gasStation.name || "",
      municipality: gasStation.municipality || "",
      address: gasStation.address || "",
      latitude: gasStation.latitude || 0,
      longitude: gasStation.longitude || 0,
    });
    setCurrentPrices({
      diesel: gasStation.dieselPrice || 0,
      petrol: gasStation.petrolPrice || 0,
      lpg: gasStation.lpgPrice || 0,
    });
    setIsFormOpen(true);
    scrollToForm();
  };

  const handleFormClose = () => {
    setEditingGasStationId(null);
    setFormData(initialFormState);
    setCurrentPrices({ diesel: 0, petrol: 0, lpg: 0 });
    setIsFormOpen(false);
  };

  return (
    <div className="bg-transparent w-full">
      <div className="flex justify-between items-center mb-4 gap-4 flex-wrap sm:flex-nowrap">
        {!isFormOpen && (
          <button
            onClick={() => {
              setIsFormOpen(true);
              scrollToForm();
            }}
            className="bg-lime-800 hover:bg-lime-700 text-white border border-lime-400 px-4 py-2 rounded text-sm font-bold transition cursor-pointer shadow-md focus-visible:outline focus-visible:outline-2 focus-visible:outline-white"
          >
            Pridėti degalinę
          </button>
        )}
      </div>

      {isFormOpen && (
        <form
          ref={formRef}
          onSubmit={handleSubmit}
          className="bg-gray-900/95 backdrop-blur-md border border-gray-700 p-5 rounded-xl space-y-4 shadow-xl mb-6 text-white"
          aria-label={editingGasStationId ? "Degalinės redagavimo forma" : "Naujos degalinės kūrimo forma"}
        >
          <h3 className="text-md font-bold text-lime-400">
            {editingGasStationId
              ? `Redaguoti informaciją: ${formData.name}`
              : "Pridėti naują degalinę"}
          </h3>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div>
              <label htmlFor="station-name" className="block text-xs font-semibold text-gray-200 mb-1">
                Degalinės pavadinimas *
              </label>
              <input
                id="station-name"
                type="text"
                name="name"
                value={formData.name}
                onChange={handleInputChange}
                required
                aria-required="true"
                className="w-full border border-gray-600 rounded px-3 py-1.5 text-sm bg-gray-800 text-white focus:outline-none focus:ring-2 focus:ring-lime-500"
              />
            </div>
            <div>
              <label htmlFor="station-municipality" className="block text-xs font-semibold text-gray-200 mb-1">
                Savivaldybė/Miestas *
              </label>
              <input
                id="station-municipality"
                type="text"
                name="municipality"
                value={formData.municipality}
                onChange={handleInputChange}
                required
                aria-required="true"
                className="w-full border border-gray-600 rounded px-3 py-1.5 text-sm bg-gray-800 text-white focus:outline-none focus:ring-2 focus:ring-lime-500"
              />
            </div>
            <div>
              <label htmlFor="station-address" className="block text-xs font-semibold text-gray-200 mb-1">
                Adresas *
              </label>
              <input
                id="station-address"
                type="text"
                name="address"
                value={formData.address}
                onChange={handleInputChange}
                required
                aria-required="true"
                className="w-full border border-gray-600 rounded px-3 py-1.5 text-sm bg-gray-800 text-white focus:outline-none focus:ring-2 focus:ring-lime-500"
              />
            </div>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4 pt-2 border-t border-gray-700">
            <div>
              <label htmlFor="station-latitude" className="block text-xs font-semibold text-gray-200 mb-1">
                Platuma (X koordinatė) *
              </label>
              <input
                id="station-latitude"
                type="number"
                step="0.000001"
                name="latitude"
                value={formData.latitude}
                onChange={handleInputChange}
                required
                aria-required="true"
                className="w-full border border-gray-600 rounded px-3 py-1.5 text-sm bg-gray-800 text-white focus:outline-none focus:ring-2 focus:ring-lime-500"
              />
            </div>
            <div>
              <label htmlFor="station-longitude" className="block text-xs font-semibold text-gray-200 mb-1">
                Ilguma (Y koordinatė) *
              </label>
              <input
                id="station-longitude"
                type="number"
                step="0.000001"
                name="longitude"
                value={formData.longitude}
                onChange={handleInputChange}
                required
                aria-required="true"
                className="w-full border border-gray-600 rounded px-3 py-1.5 text-sm bg-gray-800 text-white focus:outline-none focus:ring-2 focus:ring-lime-500"
              />
            </div>
          </div>

          <div className="flex gap-2 justify-end pt-2">
            <button
              type="button"
              onClick={handleFormClose}
              className="bg-amber-400 hover:bg-amber-300 text-black px-4 py-1.5 rounded text-sm font-bold transition cursor-pointer focus-visible:outline focus-visible:outline-2 focus-visible:outline-white"
            >
              Atšaukti
            </button>
            <button
              type="submit"
              className="bg-lime-600 hover:bg-lime-500 text-white px-4 py-1.5 rounded text-sm font-bold transition cursor-pointer focus-visible:outline focus-visible:outline-2 focus-visible:outline-white"
            >
              {editingGasStationId ? "Saugoti pakeitimus" : "Sukurti Degalinę"}
            </button>
          </div>
        </form>
      )}

      <div className="overflow-x-auto w-full rounded-xl border border-gray-300 shadow-md bg-white/95 backdrop-blur-sm">
        <table className="min-w-full divide-y divide-gray-300 text-sm">
          <thead className="bg-gray-200/90 text-gray-900 font-extrabold text-left border-b border-gray-300">
            <tr>
              <th scope="col" className="px-4 py-3 w-40">Pavadinimas</th>
              <th scope="col" className="px-4 py-3">Miestas / Adresas</th>
              <th scope="col" className="px-4 py-3 text-gray-900 font-extrabold text-xs">Platuma</th>
              <th scope="col" className="px-4 py-3 text-gray-900 font-extrabold text-xs">Ilguma</th>
              <th scope="col" className="px-4 py-3 text-center w-36">Veiksmai</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200 text-gray-800">
            {loading && gasStations.length === 0 ? (
              <tr>
                <td
                  colSpan="5"
                  aria-live="polite"
                  className="text-center py-10 font-bold text-gray-800 drop-shadow-sm bg-transparent animate-pulse"
                >
                  Užkraunamos degalinės...
                </td>
              </tr>
            ) : error ? (
              <tr>
                <td
                  colSpan="5"
                  className="text-center py-10 font-bold text-red-600 drop-shadow-sm bg-transparent"
                  role="alert"
                >
                  Klaida: {error}
                </td>
              </tr>
            ) : gasStations.length === 0 ? (
              <tr>
                <td
                  colSpan="5"
                  className="text-center py-10 text-gray-700 font-medium italic"
                >
                  Nėra užregistruotų degalinių.
                </td>
              </tr>
            ) : (
              gasStations.map((gasStation) => (
                <tr
                  key={gasStation.id}
                  className="hover:bg-lime-50/50 transition-colors"
                >
                  <td
                    className="px-4 py-3 font-bold text-gray-900 max-w-[160px] truncate"
                    title={gasStation.name}
                  >
                    {gasStation.name}
                  </td>
                  <td className="px-4 py-3">
                    <div className="font-bold text-gray-900">
                      {gasStation.municipality}
                    </div>
                    <div className="text-xs text-gray-700 font-medium mt-0.5">
                      {gasStation.address}
                    </div>
                  </td>
                  <td className="px-4 py-3 font-mono text-xs text-gray-900 font-bold">
                    {gasStation.latitude ? gasStation.latitude.toFixed(5) : "—"}
                  </td>
                  <td className="px-4 py-3 font-mono text-xs text-gray-900 font-bold">
                    {gasStation.longitude
                      ? gasStation.longitude.toFixed(5)
                      : "—"}
                  </td>
                  <td className="px-4 py-3 text-center">
                    <div className="flex gap-2 justify-center">
                      <button
                        onClick={() => handleEdit(gasStation)}
                        className="text-blue-700 hover:text-blue-900 text-xs font-bold px-2 py-1 hover:bg-blue-100 rounded transition cursor-pointer focus-visible:outline focus-visible:outline-2 focus-visible:outline-blue-700"
                        aria-label={`Redaguoti degalinę ${gasStation.name}`}
                      >
                        Redaguoti
                      </button>
                      <button
                        onClick={() =>
                          handleDelete(gasStation.id, gasStation.name)
                        }
                        className="text-red-700 hover:text-red-900 text-xs font-bold px-2 py-1 hover:bg-red-100 rounded transition cursor-pointer focus-visible:outline focus-visible:outline-2 focus-visible:outline-red-700"
                        aria-label={`Ištrinti degalinę ${gasStation.name}`}
                      >
                        Ištrinti
                      </button>
                    </div>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}