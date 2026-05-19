using KuroOpti.Data;
using KuroOpti.Entities;
using Microsoft.EntityFrameworkCore;

namespace KuroOpti.Repositories
{
    public class FuelStationRepository : IFuelStationRepository
    {
        private readonly KuroOptiDbContext _db;

        public FuelStationRepository(KuroOptiDbContext db)
        {
            _db = db;
        }

        public async Task<List<FuelStation>> GetAllAsync()
        {
            return await _db.FuelStations.AsNoTracking().ToListAsync();
        }

        public async Task<FuelStation?> GetByIdAsync(int id)
        {
            return await _db.FuelStations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(FuelStation station)
        {
            _db.FuelStations.Add(station);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(FuelStation station)
        {
            _db.FuelStations.Update(station);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var station = await _db.FuelStations.FindAsync(id);
            if (station != null)
            {
                _db.FuelStations.Remove(station);
                await _db.SaveChangesAsync();
            }
        }

        public async Task UpsertAllAsync(List<FuelStation> stations)
        {
            Dictionary<string, FuelStation> existing = await _db.FuelStations
                .ToDictionaryAsync(s => s.Name + "|" + s.Address);

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
                    _db.FuelStations.Add(incoming);
                }
            }

            foreach (KeyValuePair<string, FuelStation> kvp in existing)
            {
                if (incomingKeys.Contains(kvp.Key) == false)
                    _db.FuelStations.Remove(kvp.Value);
            }

            await _db.SaveChangesAsync();
        }
    }
}
