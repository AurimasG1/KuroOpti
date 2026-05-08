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
    }
}
