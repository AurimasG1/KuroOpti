using KuroOpti.Entities;

namespace KuroOpti.Repositories.Interfaces
{
    public interface IRoutePlanningHistoryRepository
    {
        Task AddAsync(RoutePlanningHistory history);
        Task<List<RoutePlanningHistory>> GetByUserIdAsync(int userId);
        Task<List<RoutePlanningHistory>> GetByRouteIdAsync(int routeId);
    }
}
