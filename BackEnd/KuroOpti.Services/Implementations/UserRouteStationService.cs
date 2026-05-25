using KuroOpti.Entities;
using KuroOpti.Repositories.Interfaces;
using KuroOpti.Services.Interfaces;

namespace KuroOpti.Services.Implementations
{
    public class UserRouteStationService : IUserRouteStationService
    {
        private readonly IUserRouteStationRepository userRouteStationRepository;
        private readonly IRouteRepository routeRepository;

        public UserRouteStationService(
            IRouteRepository routeRepository,
            IUserRouteStationRepository userRouteStationRepository
        )
        {
            this.routeRepository = routeRepository;
            this.userRouteStationRepository = userRouteStationRepository;
        }

        public async Task AddStationToRouteAsync(int userId, int routeId, int stationId)
        {
            var route = await routeRepository.GetByIdAsync(routeId);
            if (route == null || route.UserId != userId)
                throw new UnauthorizedAccessException("Route does not belong to this user.");

            if (await userRouteStationRepository.ExistsAsync(routeId, stationId))
                return;

            var entity = new UserRouteStation { RouteId = routeId, FuelStationId = stationId };

            await userRouteStationRepository.AddAsync(entity);
            await userRouteStationRepository.SaveChangesAsync();
        }

        public async Task RemoveStationFromRouteAsync(int userId, int routeId, int stationId)
        {
            var route = await routeRepository.GetByIdAsync(routeId);
            if (route == null || route.UserId != userId)
                throw new UnauthorizedAccessException("Route does not belong to this user.");

            await userRouteStationRepository.RemoveAsync(routeId, stationId);
            await userRouteStationRepository.SaveChangesAsync();
        }

        public async Task<List<FuelStation>> GetSelectedStationsAsync(int userId, int routeId)
        {
            var route = await routeRepository.GetByIdAsync(routeId);
            if (route == null || route.UserId != userId)
                throw new UnauthorizedAccessException("Route does not belong to this user.");

            var links = await userRouteStationRepository.GetRouteByIdAsync(routeId);
            return links.Select(x => x.FuelStation).ToList();
        }
    }
}
