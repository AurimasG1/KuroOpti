using System.Security.Claims;
using KuroOpti.API.Requests;
using KuroOpti.Entities;
using KuroOpti.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KuroOpti.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly TokenService tokenService;
        private readonly IUserService userService;

        public AuthController(TokenService tokenService, IUserService userService)
        {
            this.tokenService = tokenService;
            this.userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegistrationRequest request)
        {
            var success = await userService.RegisterAsync(request.Email, request.Password);

            if (!success)
                return BadRequest("User already exists");

            return Ok("User registered succesfully");
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
            };

            return Ok(response);
        }
    }
}
