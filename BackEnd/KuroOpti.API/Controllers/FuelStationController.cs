using KuroOpti.Entities;
using KuroOpti.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KuroOpti.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FuelStationController : ControllerBase
    {
        private readonly IFuelStationService _fuelStationService;

        public FuelStationController(IFuelStationService fuelStationService)
        {
            _fuelStationService = fuelStationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFuelStations()
        {
            var stations = await _fuelStationService.GetAllFuelStations();

            return Ok(stations);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFuelStationsById(int id)
        {
            try
            {
                var station = await _fuelStationService.GetFuelStationById(id);

                return Ok(station);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateFuelStation(FuelStation fuelStation)
        {
            var createdStation = await _fuelStationService.CreateFuelStation(fuelStation);

            return Ok(createdStation);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFuelStation(int id, FuelStation fuelStation)
        {
            try
            {
                var updateStation = await _fuelStationService.UpdateFuelStation(id, fuelStation);

                return Ok(updateStation);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFuelStation(int id)
        {
            var deleted = await _fuelStationService.DeleteFuelStation(id);

            if (!deleted)
            {
                return NotFound("Fuel station not found");
            }

            return Ok("Fuel station deleted");
        }
    }
}
