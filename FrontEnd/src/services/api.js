const BASE_URL = 'http://localhost:5211';
const API_URL = 'http://localhost:5211';

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
		Longitude: parseFloat(
			station.Longitude || station.longitude || station.lng || station.lon,
		),
		PetrolPrice: station.PetrolPrice || station.petrolPrice || 0,
		DieselPrice: station.DieselPrice || station.dieselPrice || 0,
		LpgPrice: station.LpgPrice || station.lpgPrice || 0,
	}));
};

// export const saveRouteHistory = async historyData => {
// 	const token = localStorage.getItem('accessToken');

// 	const response = await fetch(`${API_URL}/api/history`, {
// 		method: 'POST',
// 		headers: {
// 			'Content-Type': 'application/json',
// 			Authorization: `Bearer ${token}`,
// 		},
// 		body: JSON.stringify(historyData),
// 	});

// 	if (!response.ok) {
// 		throw new Error(`Serveris grąžino klaidą: ${response.status}`);
// 	}
// 	return await response.json();
// };

export const saveRouteHistory = async historyData => {
	const token = localStorage.getItem('accessToken');

	// Siunčiame istoriją fone — be await
	fetch(`${API_URL}/api/history`, {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
			Authorization: `Bearer ${token}`,
		},
		body: JSON.stringify(historyData),
	})
		.then(res => {
			if (!res.ok) {
				console.warn('Istorijos išsaugoti nepavyko:', res.status);
			}
		})
		.catch(err => {
			console.warn('Klaida siunčiant istoriją:', err);
		});
};

export const createRouteOnBackend = async (routeData, token) => {
	const response = await fetch(`${API_URL}/api/routes`, {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
			Authorization: `Bearer ${token}`,
		},
		body: JSON.stringify(routeData),
	});

	if (!response.ok) {
		const text = await response.text();
		console.error('Route creation failed:', text);
		throw new Error(`Nepavyko sukurti maršruto. Statusas: ${response.status}`);
	}

	const createdRoute = await response.json();
	console.log('Backend grąžino sukurtą maršrutą:', createdRoute);

	return createdRoute; // turi { id: ..., startLat: ..., endLat: ..., polyline: ... }
};

export const getDetailedRouteHistory = async () => {
	const token = localStorage.getItem('accessToken');

	const response = await fetch(`${API_URL}/api/history/detailed`, {
		method: 'GET',
		headers: {
			'Content-Type': 'application/json',
			Authorization: `Bearer ${token}`,
		},
	});

	if (!response.ok) {
		throw new Error('Nepavyko gauti maršruto istorijos');
	}

	return response.json();
};

export const fetchGeocode = async addr => {
	if (!addr || addr === 'Degalinė' || addr === 'Sustojimas') return null;
	try {
		const res = await fetch(
			`https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(addr + ', Lietuva')}`,
		);
		const data = await res.json();
		return data.length > 0
			? [parseFloat(data[0].lat), parseFloat(data[0].lon)]
			: null;
	} catch (err) {
		console.error('Geo-kodo klaida adresui:', addr, err);
		return null;
	}
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
