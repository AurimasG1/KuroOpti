using System.Security.Claims;
using AutoMapper;
using KuroOpti.Common.DTO;
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

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = GetUserId();
            var user = await userService.GetUserByIdAsync(userId);

            if (user == null)
                return NotFound();

            var dto = mapper.Map<UserDto>(user);
            return Ok(dto);
        }
    }
}
