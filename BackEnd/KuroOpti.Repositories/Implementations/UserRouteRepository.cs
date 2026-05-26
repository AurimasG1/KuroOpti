using KuroOpti.Data;
using KuroOpti.Entities;
using KuroOpti.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KuroOpti.Repositories.Implementations
{
    public class UserRouteStationRepository : IUserRouteStationRepository
    {
        private readonly KuroOptiDbContext db;

        public UserRouteStationRepository(KuroOptiDbContext db)
        {
            this.db = db;
        }

        public async Task AddAsync(UserRouteStation entity)
        {
            await db.UserRouteStations.AddAsync(entity);
        }

        public async Task RemoveAsync(int routeId, int stationId)
        {
            var existing = await db
                .UserRouteStations.Where(x => x.RouteId == routeId && x.FuelStationId == stationId)
                .FirstOrDefaultAsync();

            if (existing != null)
                db.UserRouteStations.Remove(existing);
        }

        public async Task<List<UserRouteStation>> GetRouteByIdAsync(int routeId)
        {
            return await db
                .UserRouteStations.AsNoTracking()
                .Include(x => x.FuelStation)
                .Where(x => x.RouteId == routeId)
                .OrderBy(x => x.AddedAt)
                .ToListAsync();
        }

        public Task<bool> ExistsAsync(int routeId, int stationId)
        {
            return db.UserRouteStations.AnyAsync(x =>
                x.RouteId == routeId && x.FuelStationId == stationId
            );
        }

        public async Task ClearStationsForRouteAsync(int routeId)
        {
            var items = await db.UserRouteStations.Where(x => x.RouteId == routeId).ToListAsync();

            if (items.Count > 0)
                db.UserRouteStations.RemoveRange(items);
        }

        public Task SaveChangesAsync() => db.SaveChangesAsync();
    }
}
