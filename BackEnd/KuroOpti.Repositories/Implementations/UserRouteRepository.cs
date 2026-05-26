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

        public async Task AddAsync(UserRouteStation userRouteStation)
        {
            await db.UserRouteStations.AddAsync(userRouteStation);
        }

        public async Task RemoveAsync(int routeId, int stationId)
        {
            var existing = await db.UserRouteStations.FirstOrDefaultAsync(x =>
                x.RouteId == routeId && x.FuelStationId == stationId
            );

            if (existing != null)
                db.UserRouteStations.Remove(existing);
        }

        public Task<List<UserRouteStation>> GetRouteByIdAsync(int routeId)
        {
            return db
                .UserRouteStations.AsNoTracking()
                .Include(x => x.FuelStation)
                .Where(x => x.RouteId == routeId)
                .ToListAsync();
        }

        public Task<bool> ExistsAsync(int routeId, int stationId)
        {
            return db.UserRouteStations.AnyAsync(x =>
                x.RouteId == routeId && x.FuelStationId == stationId
            );
        }

        public Task SaveChangesAsync() => db.SaveChangesAsync();
    }
}
