const BASE_URL = 'http://localhost:5211';
const API_URL = 'http://localhost:5211';

export const fetchPricesFromApi = async () => {
	const response = await fetch(`${BASE_URL}/api/prices/update`, {
		method: 'POST',
		headers: {
			'Content-Type': 'applications/json',
		},
	});
	if (!response.ok) {
		const text = await response.text();
		console.error('Kainų atnaujinimo klaida:', text);
		throw new Error('Nepavyko atnaujinti kainų');
	}
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

export const saveRouteHistory = async (historyData, token) => {
	const response = await fetch(`${API_URL}/api/history/list`, {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
			Authorization: `Bearer ${token}`,
		},
		body: JSON.stringify(historyData),
	});

	if (!response.ok) {
		const text = await response.text();
		console.error('Istorijos išsaugoti nepavyko', text);
		throw new Error(`Nepavyko išsaugoti istorijos. Statusas: ${response.status}`);
	}

	return await response.json();
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

	const response = await fetch(`${API_URL}/api/history/list`, {
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

export const deleteRouteHistory = async routeId => {
	const token = localStorage.getItem('accessToken');

	const response = await fetch(`${API_URL}/api/history/${routeId}`, {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json',
			Authorization: `Bearer ${token}`,
		},
	});
	if (!response.ok) {
		throw new Error('Nepavyko gauti maršruto istorijos');
	}

	return true;
};

export const clearRouteHistory = async () => {
	const token = localStorage.getItem('accessToken');

	const res = await fetch(`${API_URL}/api/history/clear`, {
		method: 'DELETE',
		headers: {
			Authorization: `Bearer ${token}`,
		},
	});

	if (!res.ok) {
		throw new Error('Nepavyko išvalyti istorijos');
	}

	return true;
};

export const getRegionPrices = async () => {
	const res = await fetch(`${BASE_URL}/api/analytics/region-prices`);
	if (!res.ok) throw new Error('Nepavyko gauti regioninių kainų');
	return await res.json();
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
