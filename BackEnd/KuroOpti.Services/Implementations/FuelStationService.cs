using AutoMapper;
using KuroOpti.Common.DTO;
using KuroOpti.Entities;
using KuroOpti.Repositories;
using KuroOpti.Services.Interfaces;

namespace KuroOpti.Services.Implementations
{
    public class FuelStationService : IFuelStationService
    {
        private readonly IFuelStationRepository repository;

        public FuelStationService(IFuelStationRepository repository)
        {
            this.repository = repository;
        }

        public async Task<List<FuelStation>> GetAllFuelStations()
        {
            return await repository.GetAllAsync();
        }

        public async Task<FuelStation> GetFuelStationById(int id)
        {
            var station = await repository.GetByIdAsync(id);

            if (station == null)
                throw new KeyNotFoundException("Fuel station not found");

            return station;
        }

        public async Task<FuelStation> CreateFuelStation(FuelStation fuelStation)
        {
            await repository.AddAsync(fuelStation);

            return fuelStation;
        }

        public async Task<FuelStation> UpdateFuelStation(int id, FuelStation fuelStation)
        {
            var existing = await repository.GetByIdAsync(id);

            if (existing == null)
            {
                throw new Exception("Fuel station not found");
            }

            existing.Name = fuelStation.Name;
            existing.Address = fuelStation.Address;
            existing.Municipality = fuelStation.Municipality;
            existing.DieselPrice = fuelStation.DieselPrice;
            existing.PetrolPrice = fuelStation.PetrolPrice;
            existing.LpgPrice = fuelStation.LpgPrice;

            await repository.UpdateAsync(existing);

            return existing;
        }

        public async Task<bool> DeleteFuelStation(int id)
        {
            var station = await repository.GetByIdAsync(id);

            if (station == null)
            {
                return false;
            }

            await repository.DeleteAsync(id);

            return true;
        }
    }
}
