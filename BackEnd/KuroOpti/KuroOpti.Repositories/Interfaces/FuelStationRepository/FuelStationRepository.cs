using KuroOpti.Entities;

namespace KuroOpti.Repositories.Interfaces.FuelStationRepository
{
    public interface FuelStationRepository : IFuelStationRepository
    {
        private readonly AppDbContext _context;

        public FuelStationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<FuelStation>> GetAllAsync()
        {
            return await _context.FuelStations.ToListAsync();
        }

        public async Task<FuelStation?> GetByIdAsync(int id)
        {
            return await _context.FuelStations.FindAsync(id);
        }

        public async Task AddAsync(FuelStation station)
        {
            _context.FuelStations.Add(station);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FuelStation station)
        {
            _context.FuelStations.Update(station);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var station = await _context.FuelStations.FindAsync(id);
            if (station != null)
            {
                _context.FuelStations.Remove(station);
                await _context.SaveChangesAsync();
            }
        }
    }
}