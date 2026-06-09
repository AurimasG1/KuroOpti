using System.Text.Json;
using KuroOpti.Common.Requests;
using KuroOpti.Entities;
using KuroOpti.Repositories;
using KuroOpti.Repositories.Interfaces;
using KuroOpti.Services.Interfaces;

namespace KuroOpti.Services.Implementations
{
    public class RoutePlanningHistoryService : IRoutePlanningHistoryService
    {
        private readonly IRoutePlanningHistoryRepository historyRepo;
        private readonly IFuelStationRepository fuelStationRepo;

        public RoutePlanningHistoryService(
            IRoutePlanningHistoryRepository historyRepo,
            IFuelStationRepository fuelStationRepo
        )
        {
            this.historyRepo = historyRepo;
            this.fuelStationRepo = fuelStationRepo;
        }

        public async Task AddHistoryAsync(int userId, SaveHistoryRequest req)
        {
            var history = new RoutePlanningHistory
            {
                UserId = userId,
                RouteId = req.RouteId,
                StartAddress = req.StartAddress,
                EndAddress = req.EndAddress,
                StartLat = req.StartLat,
                StartLng = req.StartLng,
                EndLat = req.EndLat,
                EndLng = req.EndLng,
                FuelType = req.FuelType,
                DistanceKm = req.DistanceKm,
                SelectedStationsJson = JsonSerializer.Serialize(req.SelectedStationIds),
                PlannedAt = DateTime.UtcNow,
            };

            await historyRepo.AddAsync(history);
        }

        public async Task<List<RoutePlanningHistory>> GetHistoryForUserAsync(int userId)
        {
            return await historyRepo.GetByUserIdAsync(userId);
        }

        public async Task<List<RoutePlanningHistory>> GetHistoryWithStationsAsync(int userId)
        {
            var history = await historyRepo.GetByUserIdAsync(userId);

            foreach (var item in history)
            {
                var ids = JsonSerializer.Deserialize<List<int>>(item.SelectedStationsJson)!;

                item.Stations = await fuelStationRepo.GetByIdsAsync(ids);
            }

            return history;
        }

        public async Task<List<RoutePlanningHistory>> GetHistoryForRouteAsync(
            int userId,
            int routeId
        )
        {
            var history = await historyRepo.GetByRouteIdAsync(routeId);

            return history.Where(x => x.UserId == userId).ToList();
        }
    }
}
