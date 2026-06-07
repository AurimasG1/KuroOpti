using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KuroOpti.Services.Interfaces;
using KuroOpti.Common.DTO;

namespace KuroOpti.API.Controllers
{
	[ApiController]
	[Route("api/admin")]
	public class AdminController : ControllerBase
	{
		private readonly IAdminService adminService;
		public AdminController(IAdminService adminService)
		{
			this.adminService = adminService;
		}

		[HttpGet("dashboard-stats")]
		public async Task<IActionResult> GetDashboardStats()
		{
			var totalUsers = await adminService.GetTotalUsersAsync();

			return Ok(new
			{
				TotalUsers = totalUsers,
				ServerTime = DateTime.UtcNow
			});
		}

		[HttpGet("users")]
		public async Task<IActionResult> GetAllUsers()
		{
			var users = await adminService.GetAllUsersAsync();
			return Ok(users);
		}

		[HttpPut("users/{id}")]
		public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto userDto)
		{
			if (userDto == null) return BadRequest("Invalid data");

			var result = await adminService.UpdateUserAsync(id, userDto);

			if (!result) return NotFound($"User with ID {id} not found");

			return Ok(new { message = "User updated successfully" });
		}


		[HttpDelete("users/{id}")]
		public async Task<IActionResult> DeleteUser(int id)
		{
			var result = await adminService.DeleteUserAsync(id);

			if (!result) return NotFound($"User with ID {id} was not found.");

			return Ok(new { message = "User deleted successfully." });
		}
	}
}