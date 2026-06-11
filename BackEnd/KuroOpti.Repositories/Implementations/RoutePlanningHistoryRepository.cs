using KuroOpti.Data;
using KuroOpti.Entities;
using KuroOpti.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KuroOpti.Repositories.Implementations
{
    public class RoutePlanningHistoryRepository : IRoutePlanningHistoryRepository
    {
        private readonly KuroOptiDbContext db;

        public RoutePlanningHistoryRepository(KuroOptiDbContext db)
        {
            this.db = db;
        }

        public async Task AddAsync(RoutePlanningHistory history)
        {
            await db.RoutePlanningHistories.AddAsync(history);
            await db.SaveChangesAsync();
        }

        public async Task<List<RoutePlanningHistory>> GetByUserIdAsync(int userId)
        {
            return await db
                .RoutePlanningHistories.Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<RoutePlanningHistory>> GetByRouteIdAsync(int routeId)
        {
            return await db
                .RoutePlanningHistories.Where(x => x.RouteId == routeId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task DeleteAsync(int id, int userId)
        {
            var item = await db.RoutePlanningHistories.FirstOrDefaultAsync(x =>
                x.Id == id && x.UserId == userId
            );

            if (item != null)
            {
                db.RoutePlanningHistories.Remove(item);
                await db.SaveChangesAsync();
            }
        }

        public async Task ClearAsync(int userId)
        {
            var items = await db
                .RoutePlanningHistories.Where(x => x.UserId == userId)
                .ToListAsync();

            db.RoutePlanningHistories.RemoveRange(items);
            await db.SaveChangesAsync();
        }
    }
}
