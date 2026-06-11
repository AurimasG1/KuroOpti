using System.Security.Claims;
using AutoMapper;
using KuroOpti.Common.DTO;
using KuroOpti.Common.Requests;
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

        [HttpPost("list")]
        public async Task<IActionResult> Add([FromBody] RoutePlanningHistoryDto dto)
        {
            await historyService.AddHistoryAsync(GetUserId(), dto);
            return Ok(new { message = "History saved" });
        }

        [HttpGet("list")]
        public async Task<IActionResult> List()
        {
            var result = await historyService.GetHistoryForUserAsync(GetUserId());
            return Ok(result);
        }

        [HttpGet("{routeId}")]
        public async Task<IActionResult> GetForRoute(int routeId)
        {
            var result = await historyService.GetHistoryForRouteAsync(GetUserId(), routeId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await historyService.DeleteHistoryAsync(id, GetUserId());
            return Ok(new { message = "Deleted" });
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> Clear()
        {
            await historyService.ClearHistoryAsync(GetUserId());
            return Ok(new { message = "Cleared" });
        }
    }
}
