using KuroOpti.Entities;

namespace KuroOpti.Repositories.Interfaces.FuelStationRepository
{
    public interface IFuelStationRepository
    {
        Task<List<FuelStation>> GetAllAsync();
        Task<FuelStation?> GetByIdAsync(int id);
        Task AddAsync(FuelStation station);
        Task UpdateAsync(FuelStation station);
        Task DeleteAsync(int id);
    }
}
