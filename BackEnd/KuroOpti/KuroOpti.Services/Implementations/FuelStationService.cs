using KuroOpti.Entities;
using KuroOpti.Services.Interfaces;

namespace KuroOpti.Services;

public class FuelStationService : IFuelStationService
{
    public Task<List<FuelStation>> GetAllFuelStations()
    {
        throw new NotImplementedException();
    }

    public Task<FuelStation> GetFuelStationById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<FuelStation> CreateFuelStation(FuelStation fuelStation)
    {
        throw new NotImplementedException();
    }

    public Task<FuelStation> UpdateFuelStation(int id, FuelStation fuelStation)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteFuelStation(int id)
    {
        throw new NotImplementedException();
    }
}