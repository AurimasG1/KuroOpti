using System.Security.Claims;
using AutoMapper;
using KuroOpti.API.Requests;
using KuroOpti.API.Responses;
using KuroOpti.Entities;
using KuroOpti.Services.Interfaces;
using KuroOpti.Services.Interfaces.KuroOpti.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KuroOpti.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService tokenService;
        private readonly IUserService userService;
        private readonly IMapper mapper;

        public AuthController(ITokenService tokenService, IUserService userService, IMapper mapper)
        {
            this.tokenService = tokenService;
            this.userService = userService;
            this.mapper = mapper;
        }

        private List<Claim> BuildClaims(User user)
        {
            return new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
            };
        }

        private AuthResponse BuildAuthResponse(User user)
        {
            var claims = BuildClaims(user);

            return new AuthResponse
            {
                AccessToken = tokenService.GenerateAccessToken(claims),
                RefreshToken = tokenService.GenerateRefreshToken(),
                User = mapper.Map<UserDto>(user),
            };
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegistrationRequest request)
        {
            var user = await userService.RegisterAsync(request.Email, request.Password);

            if (user == null)
                return BadRequest("User already exists");

            return Ok(BuildAuthResponse(user));
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            var user = await userService.ValidateUserAsync(request.Email, request.Password);

            if (user == null)
                return Unauthorized("Invalid credentials");

            return Ok(BuildAuthResponse(user));
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = await userService.GetUserByIdAsync(userId);

            if (user == null)
                return NotFound();

            return Ok(mapper.Map<UserDto>(user));
        }
    }
}
