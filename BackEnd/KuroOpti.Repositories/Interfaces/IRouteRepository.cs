using KuroOpti.Entities;

namespace KuroOpti.Repositories.Interfaces
{
	public interface IRouteRepository
	{
		Task<List<Route>> GetAllAsync();
		Task<Route?> GetByIdAsync(int id);
		Task AddAsync(Route route);
		Task UpdateAsync(Route route);
		Task DeleteAsync(int id);
	}
}