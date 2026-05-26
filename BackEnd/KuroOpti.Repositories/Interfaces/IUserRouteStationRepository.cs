using KuroOpti.Entities;

namespace KuroOpti.Repositories.Interfaces
{
    public interface IUserRouteStationRepository
    {
        Task AddAsync(UserRouteStation entity);
        Task RemoveAsync(int routeId, int stationId);
        Task<List<UserRouteStation>> GetRouteByIdAsync(int routeId);
        Task<bool> ExistsAsync(int routeId, int stationId);
        Task SaveChangesAsync();
    }
}
