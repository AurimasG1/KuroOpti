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

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Missing user ID claim");

            var userId = int.TryParse(userIdClaim.Value, out int id);

            var user = await userService.GetUserById(id);
            if (user == null)
                return NotFound("User not found");

            var response = mapper.Map<UserDto>(user);

            return Ok(response);
        }
    }
}
