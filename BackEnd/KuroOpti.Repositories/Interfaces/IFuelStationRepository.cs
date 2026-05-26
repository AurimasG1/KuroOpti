using KuroOpti.Entities;

namespace KuroOpti.Repositories
{
    public interface IFuelStationRepository
    {
        Task<List<FuelStation>> GetAllAsync();
        Task<FuelStation?> GetByIdAsync(int id);
        Task<List<FuelStation>> GetByIdsAsync(List<int> ids);
        Task AddAsync(FuelStation station);
        Task UpdateAsync(FuelStation station);
        Task DeleteAsync(int id);
        Task UpsertAllAsync(List<FuelStation> stations);
        Task<List<FuelStation>> GetUngeocodedAsync();
        Task UpdateCoordinatesAsync(int id, decimal latitude, decimal longitude);
    }
}
