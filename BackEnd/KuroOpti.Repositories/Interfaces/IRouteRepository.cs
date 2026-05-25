using KuroOpti.Entities;

namespace KuroOpti.Repositories.Interfaces
{
    public interface IRouteRepository
    {
        Task<Route?> GetByIdAsync(int id);
        Task<List<Route>> GetByUserIdAsync(int userId);
        Task AddAsync(Route route);
        Task DeleteAsync(Route route);
        Task SaveChangesAsync();
    }
}
