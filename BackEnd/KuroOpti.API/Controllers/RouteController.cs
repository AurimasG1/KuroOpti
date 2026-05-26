using System.Security.Claims;
using AutoMapper;
using KuroOpti.Common.DTO;
using KuroOpti.Common.Responses;
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

        // POST: api/routes
        [HttpPost]
        public async Task<IActionResult> CreateRoute([FromBody] RouteDto dto)
        {
            if (dto == null)
                return BadRequest("Route data is required");

            var userId = GetUserId();

            var route = mapper.Map<Entities.Route>(dto);

            var created = await routeService.CreateRouteAsync(userId, route);
            return Ok(created);
        }

        // GET: api/routes/my
        [HttpGet("my")]
        public async Task<IActionResult> GetRoutes()
        {
            var userId = GetUserId();
            var routes = await routeService.GetUserRoutesAsync(userId);

            return Ok(routes);
        }

        // GET: api/routes/{routeId}
        [HttpGet("{routeId}")]
        public async Task<IActionResult> GetRoute(int routeId)
        {
            var userId = GetUserId();

            var route = await routeService.GetRouteAsync(userId, routeId);

            if (route == null)
                return NotFound("Route not found");

            return Ok(route);
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
