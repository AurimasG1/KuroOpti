using AutoMapper;
using KuroOpti.Common.DTO;
using KuroOpti.Entities;
using KuroOpti.Repositories;
using KuroOpti.Repositories.Interfaces;
using KuroOpti.Services.Interfaces;

namespace KuroOpti.Services.Implementations
{
    public class UserRouteStationService : IUserRouteStationService
    {
        private readonly IRouteRepository routeRepository;
        private readonly IFuelStationRepository fuelStationRepository;
        private readonly IUserRouteStationRepository userRouteStationRepository;

        public UserRouteStationService(
            IRouteRepository routeRepository,
            IFuelStationRepository fuelStationRepository,
            IUserRouteStationRepository userRouteStationRepository
        )
        {
            this.routeRepository = routeRepository;
            this.fuelStationRepository = fuelStationRepository;
            this.userRouteStationRepository = userRouteStationRepository;
        }

        private async Task<Route> ValidateRouteOwnershipAsync(int userId, int routeId)
        {
            var route = await routeRepository.GetByIdAsync(routeId);

            if (route == null)
                throw new KeyNotFoundException("Route not found.");

            if (route.UserId != userId)
                throw new UnauthorizedAccessException("You do not own this route.");

            return route;
        }

        public async Task AddStationToRouteAsync(int userId, int routeId, int stationId)
        {
            await ValidateRouteOwnershipAsync(userId, routeId);

            // Check if station exists
            var station = await fuelStationRepository.GetByIdAsync(stationId);
            if (station == null)
                throw new KeyNotFoundException("Fuel station not found");

            // Check if already added
            if (await userRouteStationRepository.ExistsAsync(routeId, stationId))
                throw new InvalidOperationException("Station already added to this route");

            // Add
            var entity = new UserRouteStation
            {
                RouteId = routeId,
                FuelStationId = stationId,
                AddedAt = DateTime.UtcNow,
            };

            await userRouteStationRepository.AddAsync(entity);
            await userRouteStationRepository.SaveChangesAsync();
        }

        public async Task RemoveStationFromRouteAsync(int userId, int routeId, int stationId)
        {
            await ValidateRouteOwnershipAsync(userId, routeId);

            //  Check if station exists in route
            if (!await userRouteStationRepository.ExistsAsync(routeId, stationId))
                throw new InvalidOperationException("Station is not added to this route");

            // 4. Remove
            await userRouteStationRepository.RemoveAsync(routeId, stationId);
            await userRouteStationRepository.SaveChangesAsync();
        }

        public async Task<List<FuelStation>> GetSelectedStationsAsync(int userId, int routeId)
        {
            await ValidateRouteOwnershipAsync(userId, routeId);

            //  Get selected stations
            var selected = await userRouteStationRepository.GetRouteByIdAsync(routeId);

            return selected.Select(x => x.FuelStation).ToList();
        }

        public async Task ClearStationsForRouteAsync(int userId, int routeId)
        {
            await ValidateRouteOwnershipAsync(userId, routeId);

            await userRouteStationRepository.ClearStationsForRouteAsync(routeId);
            await userRouteStationRepository.SaveChangesAsync();
        }
    }
}
