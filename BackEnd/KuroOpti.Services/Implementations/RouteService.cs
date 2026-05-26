using AutoMapper;
using KuroOpti.Common.DTO;
using KuroOpti.Entities;
using KuroOpti.Repositories.Interfaces;
using KuroOpti.Services.Interfaces;

namespace KuroOpti.Services.Implementations
{
    public class RouteService : IRouteService
    {
        private readonly IRouteRepository routeRepository;
        private readonly IMapper mapper;

        public RouteService(IRouteRepository routeRepository, IMapper mapper)
        {
            this.routeRepository = routeRepository;
            this.mapper = mapper;
        }

        public async Task<RouteDto> CreateRouteAsync(int userId, Route route)
        {
            route.UserId = userId;

            await routeRepository.AddAsync(route);
            await routeRepository.SaveChangesAsync();

            return mapper.Map<RouteDto>(route);
        }

        public async Task<List<RouteDto>> GetUserRoutesAsync(int userId)
        {
            var routes = await routeRepository.GetByUserIdAsync(userId);
            return mapper.Map<List<RouteDto>>(routes);
        }

        public async Task<RouteDto?> GetRouteAsync(int userId, int routeId)
        {
            var route = await routeRepository.GetByIdAsync(routeId);

            if (route == null || route.UserId != userId)
                return null;

            return mapper.Map<RouteDto>(route);
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
