using AutoMapper;
using KuroOpti.Common.DTO;
using KuroOpti.Entities;
using KuroOpti.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KuroOpti.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FuelStationController : ControllerBase
    {
        private readonly IFuelStationService fuelStationService;

        private readonly IMapper mapper;

        public FuelStationController(IFuelStationService fuelStationService, IMapper mapper)
        {
            this.fuelStationService = fuelStationService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFuelStations()
        {
            var stations = await fuelStationService.GetAllFuelStations();

            return Ok(stations);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFuelStationById(int id)
        {
            try
            {
                var station = await fuelStationService.GetFuelStationById(id);

                return Ok(station);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateFuelStation([FromBody] FuelStationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = mapper.Map<FuelStation>(dto);
            var created = await fuelStationService.CreateFuelStation(entity);

            return CreatedAtAction(nameof(GetFuelStationById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFuelStation(int id, [FromBody] FuelStationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var entity = mapper.Map<FuelStation>(dto);
                var updated = await fuelStationService.UpdateFuelStation(id, entity);

                return Ok(updated);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFuelStation(int id)
        {
            var deleted = await fuelStationService.DeleteFuelStation(id);

            if (!deleted)
            {
                return NotFound("Fuel station not found");
            }

            return Ok("Fuel station deleted");
        }
    }
}
