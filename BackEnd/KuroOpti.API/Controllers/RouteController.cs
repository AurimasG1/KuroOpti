using System.Security.Claims;
using AutoMapper;
using KuroOpti.API.Responses;
using KuroOpti.Entities;
using KuroOpti.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KuroOpti.API.Controllers
{
    [ApiController]
    [Route("api/routes")]
    [Authorize]
    public class RouteController : ControllerBase
    {
        private readonly IRouteService routeService;
        private readonly IMapper mapper;

        public RouteController(IRouteService routeService, IMapper mapper)
        {
            this.routeService = routeService;
            this.mapper = mapper;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoute([FromBody] RouteDto dto)
        {
            var userId = GetUserId();
            var route = mapper.Map<Entities.Route>(dto);

            var created = await routeService.CreateRouteAsync(userId, route);
            return Ok(mapper.Map<RouteDto>(created));
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetRoutes()
        {
            var userId = GetUserId();
            var routes = await routeService.GetUserRoutesAsync(userId);

            return Ok(mapper.Map<List<RouteDto>>(routes));
        }

        [HttpDelete("{routeId}")]
        public async Task<IActionResult> DeleteRoute(int routeId)
        {
            var userId = GetUserId();
            await routeService.DeleteRouteAsync(userId, routeId);

            return NoContent();
        }
    }
}
