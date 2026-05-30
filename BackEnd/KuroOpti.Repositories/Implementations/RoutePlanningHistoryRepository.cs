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
                .OrderByDescending(x => x.PlannedAt)
                .ToListAsync();
        }

        public async Task<List<RoutePlanningHistory>> GetByRouteIdAsync(int routeId)
        {
            return await db
                .RoutePlanningHistories.Where(x => x.RouteId == routeId)
                .OrderByDescending(x => x.PlannedAt)
                .ToListAsync();
        }
    }
}
