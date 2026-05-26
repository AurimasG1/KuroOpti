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
        private readonly IUserRepository userRepository;
        private readonly IRouteRepository routeRepository;
        private readonly IFuelStationRepository fuelStationRepository;
        private readonly IUserRouteStationRepository userRouteStationRepository;
        private readonly IMapper mapper;

        public UserRouteStationService(
            IUserRepository userRepository,
            IRouteRepository routeRepository,
            IFuelStationRepository fuelStationRepository,
            IUserRouteStationRepository userRouteStationRepository,
            IMapper mapper
        )
        {
            this.userRepository = userRepository;
            this.routeRepository = routeRepository;
            this.fuelStationRepository = fuelStationRepository;
            this.userRouteStationRepository = userRouteStationRepository;
            this.mapper = mapper;
        }

        public async Task AddStationToRouteAsync(int userId, int routeId, int stationId)
        {
            // 1. Check if route exists
            var route = await routeRepository.GetByIdAsync(routeId);
            if (route == null)
                throw new UnauthorizedAccessException("Route not found.");

            // 2. Check if route belongs to user
            if (route.UserId != userId)
                throw new Exception("You do not have permission to modify this route");

            // 3. Check if station exists
            var station = await fuelStationRepository.GetByIdAsync(stationId);
            if (station == null)
                throw new Exception("fuel station not found");

            // 4. Check if already added
            bool exists = await userRouteStationRepository.ExistsAsync(routeId, stationId);
            if (exists)
                throw new Exception("Station already added to this route");

            // 5. Add
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
            // 1. Check if route exists
            var route = await routeRepository.GetByIdAsync(routeId);
            if (route == null)
                throw new Exception("Route not found");

            // 2. Check if route belongs to user
            if (route.UserId != userId)
                throw new Exception("You do not have permission to modify this route");

            // 3. Check if station exists in route
            bool exists = await userRouteStationRepository.ExistsAsync(routeId, stationId);
            if (!exists)
                throw new Exception("Station is not added to this route");

            // 4. Remove
            await userRouteStationRepository.RemoveAsync(routeId, stationId);
            await userRouteStationRepository.SaveChangesAsync();
        }

        public async Task<List<FuelStationDto>> GetSelectedStationsAsync(int userId, int routeId)
        {
            // 1. Check if route exists
            var route = await routeRepository.GetByIdAsync(routeId);
            if (route == null)
                throw new Exception("Route not found");

            // 2. Check if route belongs to user
            if (route.UserId != userId)
                throw new Exception("You do not have permission to view this route");

            // 3. Get selected stations
            var selected = await userRouteStationRepository.GetRouteByIdAsync(routeId);

            // 4. Map to DTO
            return selected.Select(x => mapper.Map<FuelStationDto>(x.FuelStation)).ToList();
        }
    }
}
