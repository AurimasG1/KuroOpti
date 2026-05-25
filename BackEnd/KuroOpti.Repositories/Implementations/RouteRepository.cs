using KuroOpti.Data;
using KuroOpti.Entities;
using KuroOpti.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KuroOpti.Repositories.Implementations
{
    public class RouteRepository : IRouteRepository //need route interface
    {
        private readonly KuroOptiDbContext _context;

        public RouteRepository(KuroOptiDbContext context)
        {
            _context = context;
        }

        public async Task<List<Route>> GetAllAsync()
        {
            return await _context.Routes.ToListAsync();
        }

        public async Task<Route?> GetByIdAsync(int id)
        {
            return await _context.Routes.FindAsync(id);
        }

        public async Task AddAsync(Route route)
        {
            await _context.Routes.AddAsync(route);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Route route)
        {
            _context.Routes.Update(route);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var route = await _context.Routes.FindAsync(id);

            if (route != null)
            {
                _context.Routes.Remove(route);
                await _context.SaveChangesAsync();
            }
        }
    }
}