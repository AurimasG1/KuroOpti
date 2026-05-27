using KuroOpti.Data;
using KuroOpti.Entities;
using KuroOpti.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KuroOpti.Repositories.Implementations
{
    public class RouteRepository : IRouteRepository
    {
        private readonly KuroOptiDbContext db;

        public RouteRepository(KuroOptiDbContext db)
        {
            this.db = db;
        }

        public Task<Route?> GetByIdAsync(int id)
        {
            return db.Routes.Include(r => r.SelectedStations).FirstOrDefaultAsync(r => r.Id == id);
        }

        public Task<List<Route>> GetByUserIdAsync(int userId)
        {
            return db.Routes.Where(r => r.UserId == userId).AsNoTracking().ToListAsync();
        }

        public async Task AddAsync(Route route)
        {
            await db.Routes.AddAsync(route);
        }

        public Task DeleteAsync(Route route)
        {
            db.Routes.Remove(route);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync() => db.SaveChangesAsync();
    }
}
