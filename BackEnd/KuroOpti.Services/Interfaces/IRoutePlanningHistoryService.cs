using KuroOpti.Common.Requests;
using KuroOpti.Entities;

namespace KuroOpti.Services.Interfaces
{
    public interface IRoutePlanningHistoryService
    {
        Task AddHistoryAsync(int userId, SaveHistoryRequest request);
        Task<List<RoutePlanningHistory>> GetHistoryForUserAsync(int userId);
        Task<List<RoutePlanningHistory>> GetHistoryWithStationsAsync(int userId);
        Task<List<RoutePlanningHistory>> GetHistoryForRouteAsync(int userId, int routeId);
    }
}
