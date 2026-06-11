using KuroOpti.Common.DTO;
using KuroOpti.Common.Requests;
using KuroOpti.Entities;

namespace KuroOpti.Services.Interfaces
{
    public interface IRoutePlanningHistoryService
    {
        Task<RoutePlanningHistoryDto> AddHistoryAsync(int userId, RoutePlanningHistoryDto dto);
        Task<List<RoutePlanningHistoryDto>> GetHistoryForUserAsync(int userId);
        Task<List<RoutePlanningHistoryDto>> GetHistoryForRouteAsync(int userId, int routeId);
        Task DeleteHistoryAsync(int id, int userId);
        Task ClearHistoryAsync(int userId);
    }
}
