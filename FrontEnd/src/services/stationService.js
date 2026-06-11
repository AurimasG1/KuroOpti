import * as stationApi from './api';

export const mergeStationsWithPrices = (stations, prices) => {
	if (!stations) return [];
	if (!prices || prices.length === 0) return stations;

	return stations.map(station => {
		const priceInfo = prices.find(p => p.station_id === station.id);

		return {
			...station,
			gasoline: priceInfo?.gasoline || null,
			diesel: priceInfo?.diesel || null,
			gas: priceInfo?.gas || null,
			lastUpdated: priceInfo?.updated_at || null,
		};
	});
};

export const getFullStationData = async localStations => {
	try {
		const prices = await stationApi.getPrices();

		return mergeStationsWithPrices(localStations, prices);
	} catch (error) {
		console.error('Server error: ', error);
		return localStations;
	}
};
