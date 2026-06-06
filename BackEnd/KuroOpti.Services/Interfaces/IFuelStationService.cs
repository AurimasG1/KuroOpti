using KuroOpti.Common.DTO;
using KuroOpti.Entities;

namespace KuroOpti.Services.Interfaces
{
    public interface IFuelStationService
    {
        Task<List<FuelStation>> GetAllFuelStations();
        Task<FuelStation> GetFuelStationById(int id);
        Task<FuelStation> CreateFuelStation(FuelStation fuelStation);
        Task<FuelStation> UpdateFuelStation(int id, FuelStation fuelStation);
        Task<bool> DeleteFuelStation(int id);
        Task<List<FuelStation>> GetStationsAlongRouteAsync(string polyline, string fuelType, decimal maxDistanceKm);
    }
}
