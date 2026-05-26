using System.Security.Claims;
using AutoMapper;
using KuroOpti.Common.DTO;
using KuroOpti.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KuroOpti.API.Controllers
{
    [ApiController]
    [Route("api/user-route-stations")]
    [Authorize]
    public class UserRouteStationController : ControllerBase
    {
        private readonly IUserRouteStationService userRouteStationService;
        private readonly IMapper mapper;

        public UserRouteStationController(
            IUserRouteStationService userRouteStationService,
            IMapper mapper
        )
        {
            this.userRouteStationService = userRouteStationService;
            this.mapper = mapper;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }

        public class ModifyStationRequest
        {
            public int RouteId { get; set; }
            public int StationId { get; set; }
        }

        // POST: api/user-route-stations/add
        [HttpPost("add")]
        public async Task<IActionResult> AddStation([FromBody] ModifyStationRequest request)
        {
            var userId = GetUserId();

            await userRouteStationService.AddStationToRouteAsync(
                userId,
                request.RouteId,
                request.StationId
            );

            return NoContent();
        }

        // POST: api/user-route-stations/remove
        [HttpPost("remove")]
        public async Task<IActionResult> RemoveStation([FromBody] ModifyStationRequest request)
        {
            var userId = GetUserId();

            try
            {
                await userRouteStationService.RemoveStationFromRouteAsync(
                    userId,
                    request.RouteId,
                    request.StationId
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent();
        }

        // GET: api/user-route-stations/{routeId}
        [HttpGet("{routeId}")]
        public async Task<IActionResult> GetSelectedStations(int routeId)
        {
            var userId = GetUserId();

            var stations = await userRouteStationService.GetSelectedStationsAsync(userId, routeId);

            var dto = mapper.Map<List<FuelStationDto>>(stations);

            return Ok(dto);
        }

        // DELETE: api/user-route-stations/{routeId}/clear
        [HttpDelete("{routeId}/clear")]
        public async Task<IActionResult> ClearStations(int routeId)
        {
            var userId = GetUserId();

            await userRouteStationService.ClearStationsForRouteAsync(userId, routeId);

            return NoContent();
        }
    }
}
