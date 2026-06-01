using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KuroOpti.Services.Interfaces;

namespace KuroOpti.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = "admin")]
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
	}
}