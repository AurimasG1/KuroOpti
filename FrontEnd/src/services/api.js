
// const BASE_URL = 'http://localhost:5211'

// export const fetchPricesFromApi = async () =>{
//     const response = await fetch(`${BASE_URL}/prices`);
//     if(!response.ok) throw new Error('Network is not responding');
//     return await response.json();
// }

// export const getStations = async () => {
//     const response = await fetch(`${BASE_URL}/fuel-station`);
//     if(!response.ok) throw new Error('Network is not responding');
//     return await response.json();
// }


// Laikinas testavimui 
import stationsData from "../data/data.json";  

export const getStations = async () => {
    try { 
        return new Promise((resolve) => {
            setTimeout(() => {
                resolve(stationsData);
            }, 300);
        });
    } catch (error) {
        console.error("Klaida imituojant API užklausą:", error);
        throw error;
    }
};

export const fetchPricesFromApi = async () => { 
    return [];
};