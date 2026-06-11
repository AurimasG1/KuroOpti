using System.Text.Json;
using KuroOpti.Common.DTO;
using KuroOpti.Entities;
using KuroOpti.Repositories.Interfaces;
using KuroOpti.Services.Interfaces;

namespace KuroOpti.Services.Implementations
{
    public class RoutePlanningHistoryService : IRoutePlanningHistoryService
    {
        private readonly IRoutePlanningHistoryRepository historyRepo;

        public RoutePlanningHistoryService(IRoutePlanningHistoryRepository historyRepo)
        {
            this.historyRepo = historyRepo;
        }

        // ---------------------------------------------------------
        // IŠSAUGOTI ISTORIJĄ
        // ---------------------------------------------------------
        public async Task<RoutePlanningHistoryDto> AddHistoryAsync(
            int userId,
            RoutePlanningHistoryDto dto
        )
        {
            var entity = new RoutePlanningHistory
            {
                UserId = userId,
                RouteId = dto.RouteId,
                StartAddress = dto.StartAddress,
                EndAddress = dto.EndAddress,
                StartLat = dto.StartLat,
                StartLng = dto.StartLng,
                EndLat = dto.EndLat,
                EndLng = dto.EndLng,
                FuelType = dto.FuelType,
                Polyline = dto.Polyline,
                CreatedAt = DateTime.UtcNow,
            };

            // Stotelės → JSON
            entity.StationsJson = JsonSerializer.Serialize(dto.Stations);

            // Tikras polyline atstumas
            entity.DistanceKm = CalculatePolylineDistance(dto.Polyline);

            // Kuro sąmata
            entity.FuelEstimate = CalculateFuelEstimate(entity.DistanceKm, entity.FuelType);

            // Išsaugome DB
            await historyRepo.AddAsync(entity);

            // Grąžiname DTO
            dto.Id = entity.Id;
            dto.DistanceKm = entity.DistanceKm;
            dto.FuelEstimate = entity.FuelEstimate;

            return dto;
        }

        // ---------------------------------------------------------
        // GAUTI ISTORIJĄ PAGAL USER
        // ---------------------------------------------------------
        public async Task<List<RoutePlanningHistoryDto>> GetHistoryForUserAsync(int userId)
        {
            var items = await historyRepo.GetByUserIdAsync(userId);

            return items.Select(MapToDto).ToList();
        }

        // ---------------------------------------------------------
        // GAUTI ISTORIJĄ PAGAL ROUTE
        // ---------------------------------------------------------
        public async Task<List<RoutePlanningHistoryDto>> GetHistoryForRouteAsync(
            int userId,
            int routeId
        )
        {
            var items = await historyRepo.GetByRouteIdAsync(routeId);

            return items.Where(x => x.UserId == userId).Select(MapToDto).ToList();
        }

        // ---------------------------------------------------------
        // IŠTRINTI VIENĄ ĮRAŠĄ
        // ---------------------------------------------------------
        public async Task DeleteHistoryAsync(int id, int userId)
        {
            await historyRepo.DeleteAsync(id, userId);
        }

        // ---------------------------------------------------------
        // IŠVALYTI VISĄ ISTORIJĄ
        // ---------------------------------------------------------
        public async Task ClearHistoryAsync(int userId)
        {
            await historyRepo.ClearAsync(userId);
        }

        // ---------------------------------------------------------
        // ENTITY → DTO MAPPING
        // ---------------------------------------------------------
        private RoutePlanningHistoryDto MapToDto(RoutePlanningHistory item)
        {
            return new RoutePlanningHistoryDto
            {
                Id = item.Id,
                RouteId = item.RouteId,
                StartAddress = item.StartAddress,
                EndAddress = item.EndAddress,
                StartLat = item.StartLat,
                StartLng = item.StartLng,
                EndLat = item.EndLat,
                EndLng = item.EndLng,
                FuelType = item.FuelType,
                DistanceKm = item.DistanceKm,
                Polyline = item.Polyline,
                FuelEstimate = item.FuelEstimate,
                CreatedAt = item.CreatedAt,

                Stations = string.IsNullOrWhiteSpace(item.StationsJson)
                    ? new List<FuelStationDto>()
                    : JsonSerializer.Deserialize<List<FuelStationDto>>(item.StationsJson),
            };
        }

        // ---------------------------------------------------------
        // POLYLINE DISTANCE
        // ---------------------------------------------------------
        private double CalculatePolylineDistance(string polyline)
        {
            var coords = GooglePolyline.Decode(polyline);
            double totalKm = 0;

            for (int i = 1; i < coords.Count; i++)
            {
                totalKm += Haversine(
                    coords[i - 1].Latitude,
                    coords[i - 1].Longitude,
                    coords[i].Latitude,
                    coords[i].Longitude
                );
            }

            return totalKm;
        }

        private double Haversine(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371;

            var dLat = ToRad(lat2 - lat1);
            var dLon = ToRad(lon2 - lon1);

            lat1 = ToRad(lat1);
            lat2 = ToRad(lat2);

            var a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                + Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }

        private double ToRad(double deg) => deg * (Math.PI / 180);

        // ---------------------------------------------------------
        // KURO SĄMATA
        // ---------------------------------------------------------
        private double CalculateFuelEstimate(double distanceKm, string fuelType)
        {
            double consumption = fuelType switch
            {
                "petrol" => 7.0,
                "diesel" => 6.0,
                "lpg" => 9.0,
                _ => 7.0,
            };

            double liters = (distanceKm / 100.0) * consumption;

            double price = fuelType switch
            {
                "petrol" => 1.55,
                "diesel" => 1.45,
                "lpg" => 0.70,
                _ => 1.50,
            };

            return liters * price;
        }
    }

    public static class GooglePolyline
    {
        public static List<(double Latitude, double Longitude)> Decode(string polyline)
        {
            var poly = new List<(double, double)>();
            int index = 0,
                len = polyline.Length;
            int lat = 0,
                lng = 0;

            while (index < len)
            {
                int b,
                    shift = 0,
                    result = 0;
                do
                {
                    b = polyline[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);

                int dlat = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                lat += dlat;

                shift = 0;
                result = 0;

                do
                {
                    b = polyline[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);

                int dlng = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                lng += dlng;

                poly.Add((lat / 1E5, lng / 1E5));
            }

            return poly;
        }
    }
}
