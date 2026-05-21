using KuroOpti.Entities;
using KuroOpti.Repositories.Interfaces;

namespace KuroOpti.Services.Implementations
{
    public class UserRouteStationService : IUserRouteStationService
    {
        private readonly IUserRouteStationRepository _routeRepository;
        private readonly IUserRouteStationRepository _userRouteStationRepository;

        public UserRouteStationService(
            IRouteRepository routeRepository,
            IUserRouteStationRepository userRouteStationRepository
        )
        {
            _routeRepository = routeRepository;
            _userRouteStationRepository = userRouteStationRepository;
        }

        public async Task AddStationToRouteAsync(int userId, int routeId, int stationId)
        {
            var route = await _routeRepository.GetByIdAsync(routeId);
            if (route == null || route.UserId != userId)
                throw new UnauthorizedAccessException("Route does not belong to this user.");

            if (await _userRouteStationRepository.ExistsAsync(routeId, stationId))
                return;

            var entity = new UserRouteStation { RouteId = routeId, FuelStationId = stationId };

            await _userRouteStationRepository.AddAsync(entity);
            await _userRouteStationRepository.SaveChangesAsync();
        }

        public async Task RemoveStationFromRouteAsync(int userId, int routeId, int stationId)
        {
            var route = await _routeRepository.GetByIdAsync(routeId);
            if (route == null || route.UserId != userId)
                throw new UnauthorizedAccessException("Route does not belong to this user.");

            await _userRouteStationRepository.RemoveAsync(routeId, stationId);
            await _userRouteStationRepository.SaveChangesAsync();
        }

        public async Task<List<FuelStation>> GetSelectedStationsAsync(int userId, int routeId)
        {
            var route = await _routeRepository.GetByIdAsync(routeId);
            if (route == null || route.UserId != userId)
                throw new UnauthorizedAccessException("Route does not belong to this user.");

            var links = await _userRouteStationRepository.GetByRouteAsync(routeId);
            return links.Select(x => x.FuelStation).ToList();
        }
    }
}
