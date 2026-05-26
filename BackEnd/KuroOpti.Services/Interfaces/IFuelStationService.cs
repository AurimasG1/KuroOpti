using KuroOpti.Common.DTO;
using KuroOpti.Entities;

namespace KuroOpti.Services.Interfaces
{
    public interface IFuelStationService
    {
        Task<List<FuelStationDto>> GetAllFuelStations();
        Task<FuelStationDto> GetFuelStationById(int id);
        Task<FuelStationDto> CreateFuelStation(FuelStation fuelStation);
        Task<FuelStationDto> UpdateFuelStation(int id, FuelStation fuelStation);
        Task<bool> DeleteFuelStation(int id);
    }
}
