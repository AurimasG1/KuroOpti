using KuroOpti.Data;
using KuroOpti.Entities;
using Microsoft.EntityFrameworkCore;

namespace KuroOpti.Repositories
{
    public class FuelStationRepository : IFuelStationRepository
    {
        private readonly KuroOptiDbContext db;

        public FuelStationRepository(KuroOptiDbContext db)
        {
            this.db = db;
        }

        public async Task<List<FuelStation>> GetAllAsync()
        {
            return await db.FuelStations.AsNoTracking().ToListAsync();
        }

        public async Task<FuelStation?> GetByIdAsync(int id)
        {
            return await db.FuelStations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<FuelStation>> GetByIdsAsync(List<int> ids)
        {
            return await db
                .FuelStations.AsNoTracking()
                .Where(x => ids.Contains(x.Id))
                .ToListAsync();
        }

        public async Task AddAsync(FuelStation station)
        {
            db.FuelStations.Add(station);
            await db.SaveChangesAsync();
        }

        public async Task UpdateAsync(FuelStation station)
        {
            db.FuelStations.Update(station);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var station = await db.FuelStations.FindAsync(id);
            if (station != null)
            {
                db.FuelStations.Remove(station);
                await db.SaveChangesAsync();
            }
        }

        public async Task UpsertAllAsync(List<FuelStation> stations)
        {
            Dictionary<string, FuelStation> existing = await db.FuelStations.ToDictionaryAsync(s =>
                s.Name + "|" + s.Address
            );

            HashSet<string> incomingKeys = new();

            foreach (FuelStation incoming in stations)
            {
                string key = incoming.Name + "|" + incoming.Address;
                incomingKeys.Add(key);

                if (existing.TryGetValue(key, out FuelStation? dbStation))
                {
                    dbStation.Municipality = incoming.Municipality;
                    dbStation.DieselPrice = incoming.DieselPrice;
                    dbStation.PetrolPrice = incoming.PetrolPrice;
                    dbStation.LpgPrice = incoming.LpgPrice;
                    dbStation.UpdatedAt = incoming.UpdatedAt;
                }
                else
                {
                    db.FuelStations.Add(incoming);
                }
            }

            foreach (KeyValuePair<string, FuelStation> kvp in existing)
            {
                if (incomingKeys.Contains(kvp.Key) == false)
                    db.FuelStations.Remove(kvp.Value);
            }

            await db.SaveChangesAsync();
        }

        public async Task<List<FuelStation>> GetUngeocodedAsync()
        {
            return await db
                .FuelStations.Where(s => s.Latitude == 0 && s.Longitude == 0)
                .ToListAsync();
        }

        public async Task UpdateCoordinatesAsync(int id, decimal latitude, decimal longitude)
        {
            FuelStation? station = await db.FuelStations.FindAsync(id);
            if (station == null)
                return;

            station.Latitude = latitude;
            station.Longitude = longitude;
            await db.SaveChangesAsync();
        }
    }
}
