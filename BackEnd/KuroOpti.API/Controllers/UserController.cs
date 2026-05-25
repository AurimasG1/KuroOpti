using System.Security.Claims;
using AutoMapper;
using KuroOpti.API.Responses;
using KuroOpti.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KuroOpti.API.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IUserRouteStationService userRouteStationService;
        private readonly IMapper mapper;

        public UserController(
            IUserService userService,
            IMapper mapper,
            IUserRouteStationService userRouteStationService
        )
        {
            this.userService = userService;
            this.mapper = mapper;
            this.userRouteStationService = userRouteStationService;
        }

        private int GetUserId()
        {
            var idClaim =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("id")?.Value;

            return int.Parse(idClaim!);
        }

        public class ModifyStationRequest
        {
            public int RouteId { get; set; }
            public int StationId { get; set; }
        }

        [HttpPost("route/stations/add")]
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

        [HttpPost("route/stations/remove")]
        public async Task<IActionResult> RemoveStation([FromBody] ModifyStationRequest request)
        {
            var userId = GetUserId();
            await userRouteStationService.RemoveStationFromRouteAsync(
                userId,
                request.RouteId,
                request.StationId
            );
            return NoContent();
        }

        [HttpGet("route/{routeId}/stations")]
        public async Task<IActionResult> GetSelectedStations(int routeId)
        {
            var userId = GetUserId();
            var stations = await userRouteStationService.GetSelectedStationsAsync(userId, routeId);
            var dto = mapper.Map<List<FuelStationDto>>(stations);
            return Ok(dto);
        }
    }
}
