using AutoMapper;
using KuroOpti.Common.DTO;
using KuroOpti.Entities;
using KuroOpti.Repositories;
using KuroOpti.Services.Interfaces;

namespace KuroOpti.Services.Implementations
{
    public class FuelStationService : IFuelStationService
    {
        private readonly IFuelStationRepository repository;

        public FuelStationService(IFuelStationRepository repository)
        {
            this.repository = repository;
        }

        public async Task<List<FuelStation>> GetAllFuelStations()
        {
            return await repository.GetAllAsync();
        }

        public async Task<FuelStation> GetFuelStationById(int id)
        {
            var station = await repository.GetByIdAsync(id);

            if (station == null)
                throw new KeyNotFoundException("Fuel station not found");

            return station;
        }

        public async Task<FuelStation> CreateFuelStation(FuelStation fuelStation)
        {
            await repository.AddAsync(fuelStation);

            return fuelStation;
        }

        public async Task<FuelStation> UpdateFuelStation(int id, FuelStation fuelStation)
        {
            var existing = await repository.GetByIdAsync(id);

            if (existing == null)
            {
                throw new Exception("Fuel station not found");
            }

            existing.Name = fuelStation.Name;
            existing.Address = fuelStation.Address;
            existing.Municipality = fuelStation.Municipality;
            existing.DieselPrice = fuelStation.DieselPrice;
            existing.PetrolPrice = fuelStation.PetrolPrice;
            existing.LpgPrice = fuelStation.LpgPrice;

            await repository.UpdateAsync(existing);

            return existing;
        }

        public async Task<bool> DeleteFuelStation(int id)
        {
            var station = await repository.GetByIdAsync(id);

            if (station == null)
            {
                return false;
            }

            await repository.DeleteAsync(id);

            return true;
        }

        public async Task<List<FuelStation>> GetStationsAlongRouteAsync(string polyline, string fuelType, decimal maxDistanceKm)
        {
            var allStations = await repository.GetAllAsync();

            var routePoints = DecodePolyline(polyline);

            var nearby = allStations
                .Where(s => s.Latitude != 0 && s.Longitude != 0)
                .Where(s => MinDistanceToPolylineKm((double)s.Latitude, (double)s.Longitude, routePoints) <= (double)maxDistanceKm)
                .ToList();

            return fuelType.ToLower() switch
            {
                "diesel" => nearby.Where(s => s.DieselPrice > 0).OrderBy(s => s.DieselPrice).ToList(),
                "petrol" => nearby.Where(s => s.PetrolPrice > 0).OrderBy(s => s.PetrolPrice).ToList(),
                "lpg" => nearby.Where(s => s.LpgPrice > 0).OrderBy(s => s.LpgPrice).ToList(),
                _ => nearby.OrderBy(s => Math.Min(s.DieselPrice > 0 ? (double)s.DieselPrice : double.MaxValue,
                                                          Math.Min(s.PetrolPrice > 0 ? (double)s.PetrolPrice : double.MaxValue,
                                                                   s.LpgPrice > 0 ? (double)s.LpgPrice : double.MaxValue))).ToList()
            };
        }

        private static List<(double Lat, double Lng)> DecodePolyline(string polyline)
        {
            var points = new List<(double, double)>();
            int index = 0, lat = 0, lng = 0;

            while (index < polyline.Length)
            {
                int b, shift = 0, result = 0;
                do
                {
                    b = polyline[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 32);
                lat += (result & 1) != 0 ? ~(result >> 1) : result >> 1;

                shift = 0;
                result = 0;
                do
                {
                    b = polyline[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 32);
                lng += (result & 1) != 0 ? ~(result >> 1) : result >> 1;

                points.Add((lat / 1e5, lng / 1e5));
            }

            return points;
        }

        private static double MinDistanceToPolylineKm(double lat, double lng, List<(double Lat, double Lng)> points)
        {
            double min = double.MaxValue;
            foreach (var point in points)
            {
                double d = HaversineDistanceKm(lat, lng, point.Lat, point.Lng);
                if (d < min) min = d;
            }
            return min;
        }

        private static double HaversineDistanceKm(double lat1, double lng1, double lat2, double lng2)
        {
            const double R = 6371;
            double dLat = ToRad(lat2 - lat1);
            double dLng = ToRad(lng2 - lng1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                       Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        private static double ToRad(double deg) => deg * Math.PI / 180;
    }
}
