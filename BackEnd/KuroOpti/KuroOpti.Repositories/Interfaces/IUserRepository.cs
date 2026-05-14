using KuroOpti.Entities;

namespace KuroOpti.Repositories.Interfaces
{
	public interface IUserRepository
	{
		Task<int> CreateAsync(User user);
		Task<User> GetByIdAsync(int id);
		Task UpdateAsync(User user);
		Task DeleteAsync(int id);
		Task<List<User>> GetAllAsync(int page, int itemsPerPage);

	}
}
