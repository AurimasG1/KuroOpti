using KuroOpti.Common.DTO;

namespace KuroOpti.Services.Interfaces
{
	public interface IAdminService
	{
		Task<int> GetTotalUsersAsync();
		Task<List<UserDto>> GetAllUsersAsync();
	}
}
