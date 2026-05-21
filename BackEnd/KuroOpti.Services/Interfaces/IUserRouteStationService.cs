using KuroOpti.Entities;

namespace KuroOpti.Services.Interfaces
{
    public interface IUserRouteStationService
    {
        Task AddStationToRouteAsync(int userId, int routeId, int stationId);
        Task RemoveStationFromRouteAsync(int userId, int routeId, int stationId);
        Task<List<FuelStation>> GetSelectedStationsAsync(int userId, int routeId);
    }
}
