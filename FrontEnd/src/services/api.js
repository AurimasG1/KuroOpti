
const BASE_URL = 'http://localhost:5211';

export const fetchPricesFromApi = async () => {
    const response = await fetch(`${BASE_URL}/api/prices`);
    if (!response.ok) throw new Error('Network is not responding');
    return await response.json();
};

export const getStations = async () => {
    const response = await fetch(`${BASE_URL}/api/fuelstation`);
    
    if (!response.ok) throw new Error('Network is not responding');
    const data = await response.json();

    return data.map(station => ({
        ...station,
        Id: station.Id || station.id,
        Name: station.Name || station.name,
        Address: station.Address || station.address,
        Latitude: parseFloat(station.Latitude || station.latitude || station.lat),
        Longitude: parseFloat(station.Longitude || station.longitude || station.lng || station.lon),
        PetrolPrice: station.PetrolPrice || station.petrolPrice || 0,
        DieselPrice: station.DieselPrice || station.dieselPrice || 0,
        LpgPrice: station.LpgPrice || station.lpgPrice || 0
    }));
};


// Laikinas testavimui 
// import stationsData from "../data/data.json";  

// export const getStations = async () => {
//     try { 
//         return new Promise((resolve) => {
//             setTimeout(() => {
//                 resolve(stationsData);
//             }, 300);
//         });
//     } catch (error) {
//         console.error("Klaida imituojant API užklausą:", error);
//         throw error;
//     }
// };

// export const fetchPricesFromApi = async () => { 
//     return [];
// };