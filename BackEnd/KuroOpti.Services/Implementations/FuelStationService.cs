using KuroOpti.Entities;
using KuroOpti.Repositories;
using KuroOpti.Services.Interfaces;

namespace KuroOpti.Services
{
    public class FuelStationService : IFuelStationService
    {
        private readonly IFuelStationRepository _repository;

        public FuelStationService(IFuelStationRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<FuelStation>> GetAllFuelStations()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<FuelStation> GetFuelStationById(int id)
        {
            var station = await _repository.GetByIdAsync(id);

            if (station == null)
            {
                throw new Exception("Fuel station not found");
            }

            return station;
        }

        public async Task<FuelStation> CreateFuelStation(FuelStation fuelStation)
        {
            await _repository.AddAsync(fuelStation);

            return fuelStation;
        }

        public async Task<FuelStation> UpdateFuelStation(int id, FuelStation fuelStation)
        {
            var existingStation = await _repository.GetByIdAsync(id);

            if (existingStation == null)
            {
                throw new Exception("Fuel station not found");
            }

            existingStation.Name = fuelStation.Name;
            existingStation.Address = fuelStation.Address;

            await _repository.UpdateAsync(existingStation);

            return existingStation;
        }

        public async Task<bool> DeleteFuelStation(int id)
        {
            var station = await _repository.GetByIdAsync(id);

            if (station == null)
            {
                return false;
            }

            await _repository.DeleteAsync(id);

            return true;
        }
    }
}