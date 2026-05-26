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
        private readonly IMapper mapper;

        public FuelStationService(IFuelStationRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<List<FuelStationDto>> GetAllFuelStations()
        {
            var stations = await repository.GetAllAsync();
            return mapper.Map<List<FuelStationDto>>(stations);
        }

        public async Task<FuelStationDto> GetFuelStationById(int id)
        {
            var station = await repository.GetByIdAsync(id);

            if (station == null)
                throw new Exception("Fuel station not found");

            return mapper.Map<FuelStationDto>(station);
        }

        public async Task<FuelStationDto> CreateFuelStation(FuelStation fuelStation)
        {
            await repository.AddAsync(fuelStation);

            return mapper.Map<FuelStationDto>(fuelStation);
        }

        public async Task<FuelStationDto> UpdateFuelStation(int id, FuelStation fuelStation)
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

            return mapper.Map<FuelStationDto>(existing);
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
