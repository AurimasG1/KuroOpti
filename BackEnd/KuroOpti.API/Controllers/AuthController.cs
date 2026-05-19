using System.Security.Claims;
using AutoMapper;
using KuroOpti.API.Requests;
using KuroOpti.API.Responses;
using KuroOpti.Entities;
using KuroOpti.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KuroOpti.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly TokenService tokenService;
        private readonly IUserService userService;
        private readonly IMapper mapper;

        public AuthController(TokenService tokenService, IUserService userService, IMapper mapper)
        {
            this.tokenService = tokenService;
            this.userService = userService;
            this.mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegistrationRequest request)
        {
            var user = await userService.RegisterAsync(request.Email, request.Password);

            if (user == null)
                return BadRequest("User already exists");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
            };
            var response = new
            {
                AccessToken = tokenService.GenerateAccessToken(claims),
                RefreshToken = tokenService.GenerateRefreshToken(),
                User = mapper.Map<UserDto>(user),
            };

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            var user = await userService.ValidateUserAsync(request.Email, request.Password);

            if (user == null)
                return Unauthorized("Invalid credentials");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var response = new
            {
                AccessToken = tokenService.GenerateAccessToken(claims),
                RefreshToken = tokenService.GenerateRefreshToken(),
                User = mapper.Map<UserDto>(user),
            };

            return Ok(response);
        }
    }
}
