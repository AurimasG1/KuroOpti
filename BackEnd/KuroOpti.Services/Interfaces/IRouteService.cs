using KuroOpti.Entities;

namespace KuroOpti.Services.Interfaces
{
    public interface IRouteService
    {
        Task<Route> CreateRouteAsync(int userId, Route route);
        Task<List<Route>> GetUserRoutesAsync(int userId);
        Task<Route?> GetRouteAsync(int userId, int routeId);
        Task DeleteRouteAsync(int userId, int routeId);
    }
}
