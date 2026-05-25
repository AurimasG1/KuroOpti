using KuroOpti.Entities;
using KuroOpti.Repositories.Interfaces;
using KuroOpti.Services.Interfaces;

namespace KuroOpti.Services.Implementations
{
    public class RouteService : IRouteService
    {
        private readonly IRouteRepository routeRepository;

        public RouteService(IRouteRepository routeRepository)
        {
            this.routeRepository = routeRepository;
        }

        public async Task<Route> CreateRouteAsync(int userId, Route route)
        {
            route.UserId = userId;

            await routeRepository.AddAsync(route);
            await routeRepository.SaveChangesAsync();

            return route;
        }

        public Task<List<Route>> GetUserRoutesAsync(int userId)
        {
            return routeRepository.GetByUserIdAsync(userId);
        }

        public async Task<Route?> GetRouteAsync(int userId, int routeId)
        {
            var route = await routeRepository.GetByIdAsync(routeId);

            if (route == null || route.UserId != userId)
                return null;

            return route;
        }

        public async Task DeleteRouteAsync(int userId, int routeId)
        {
            var route = await routeRepository.GetByIdAsync(routeId);

            if (route == null || route.UserId != userId)
                throw new UnauthorizedAccessException("Route does not belong to user");

            await routeRepository.DeleteAsync(route);
            await routeRepository.SaveChangesAsync();
        }
    }
}
