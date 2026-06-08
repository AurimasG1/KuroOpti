using System.Security.Claims;
using AutoMapper;
using KuroOpti.Common.DTO;
using KuroOpti.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KuroOpti.API.Controllers
{
    [ApiController]
    [Route("api/history")]
    [Authorize]
    public class RoutePlanningHistoryController : ControllerBase
    {
        private readonly IRoutePlanningHistoryService historyService;
        private readonly IMapper mapper;

        public RoutePlanningHistoryController(
            IRoutePlanningHistoryService historyService,
            IMapper mapper
        )
        {
            this.historyService = historyService;
            this.mapper = mapper;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }

        [HttpPost]
        public async Task<IActionResult> AddHistory([FromBody] AddHistoryRequest request)
        {
            var userId = GetUserId();
            await historyService.AddHistoryAsync(
                userId,
                request.RouteId,
                request.SelectedStationIds
            );

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetHistory()
        {
            var userId = GetUserId();
            var history = await historyService.GetHistoryForUserAsync(userId);
            var dto = mapper.Map<List<RoutePlanningHistoryDto>>(history);

            return Ok(dto);
        }

        [HttpGet("detailed")]
        public async Task<IActionResult> GetHistoryWithStations()
        {
            var userId = GetUserId();
            var history = await historyService.GetHistoryWithStationsAsync(userId);
            var dto = mapper.Map<List<RoutePlanningHistoryWithStationsDto>>(history);

            return Ok(dto);
        }

        [HttpGet("{routeId}")]
        public async Task<IActionResult> GetHistoryForRoute(int routeId)
        {
            var userId = GetUserId();
            var history = await historyService.GetHistoryForRouteAsync(userId, routeId);
            var dto = mapper.Map<List<RoutePlanningHistoryDto>>(history);

            return Ok(dto);
        }
    }
}