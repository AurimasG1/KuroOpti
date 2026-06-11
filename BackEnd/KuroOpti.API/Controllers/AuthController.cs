using System.Security.Claims;
using AutoMapper;
using KuroOpti.Common.DTO;
using KuroOpti.Common.Requests;
using KuroOpti.Common.Responses;
using KuroOpti.Entities;
using KuroOpti.Services.Implementations;
using KuroOpti.Services.Interfaces;
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
        private readonly IRefreshTokenService refreshTokenService;
        private readonly IPasswordResetService passwordResetService;
        private readonly IMapper mapper;

        public AuthController(
            ITokenService tokenService,
            IUserService userService,
            IRefreshTokenService refreshTokenService,
            IPasswordResetService passwordResetService,
            IMapper mapper
        )
        {
            this.tokenService = tokenService;
            this.userService = userService;
            this.refreshTokenService = refreshTokenService;
            this.passwordResetService = passwordResetService;
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

        private async Task<AuthResponse> BuildAuthResponse(User user)
        {
            var claims = BuildClaims(user);

            return new AuthResponse
            {
                AccessToken = tokenService.GenerateAccessToken(claims),
                RefreshToken = await refreshTokenService.CreateRefreshTokenAsync(user.Id),
                User = mapper.Map<UserDto>(user),
            };
        }

        // POST: api/auth/register
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegistrationRequest request)
        {
            var user = await userService.RegisterAsync(
                request.Email,
                request.Password,
                request.AdminCode
            );

            if (user == null)
                return BadRequest("User already exists");

            return Ok(await BuildAuthResponse(user));
        }

        // POST: api/auth/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            var user = await userService.ValidateUserAsync(request.Email, request.Password);

            if (user == null)
                return Unauthorized("Invalid credentials");

            return Ok(await BuildAuthResponse(user));
        }

        // GET: api/auth/me
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

        // PUT: api/auth/change-email
        [Authorize]
        [HttpPut("change-email")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailRequestDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            bool success = await userService.ChangeEmailAsync(userId, dto.NewEmail);

            if (!success)
                return BadRequest("Email already in use");

            return Ok("Email updated");
        }

        // PUT: api/auth/change-password
        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            bool success = await userService.ChangePasswordAsync(
                userId,
                dto.OldPassword,
                dto.NewPassword
            );

            if (!success)
                return BadRequest("Old password is incorrect");

            return Ok("Password updated");
        }

        // DELETE: api/auth/delete
        [Authorize]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAccount()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await userService.DeleteUserAsync(userId);

            return NoContent();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto)
        {
            var user = await refreshTokenService.ValidateRefreshTokenAsync(dto.RefreshToken);

            if (user == null)
                return Unauthorized("Invalid refresh token");

            var claims = BuildClaims(user);

            var newAccessToken = tokenService.GenerateAccessToken(claims);
            var newRefreshToken = await refreshTokenService.CreateRefreshTokenAsync(user.Id);

            return Ok(
                new AuthResponse { AccessToken = newAccessToken, RefreshToken = newRefreshToken }
            );
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest("Email is required");

            var baseUrl = "http://localhost:5173/reset-password";

            await passwordResetService.RequestPasswordResetAsync(request.Email, baseUrl);

            return Ok("If this email exists, a reset link was sent.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var success = await passwordResetService.ResetPasswordAsync(
                request.Token,
                request.NewPassword
            );

            if (!success)
                return BadRequest("Invalid or expired token");

            return Ok("Password updated successfully");
        }
    }
}
