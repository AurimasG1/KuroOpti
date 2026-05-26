using KuroOpti.Common.DTO;
using KuroOpti.Entities;

namespace KuroOpti.Services.Interfaces
{
    public interface IRouteService
    {
        Task<RouteDto> CreateRouteAsync(int userId, Route route);
        Task<List<RouteDto>> GetUserRoutesAsync(int userId);
        Task<RouteDto?> GetRouteAsync(int userId, int routeId);
        Task DeleteRouteAsync(int userId, int routeId);
    }
}
