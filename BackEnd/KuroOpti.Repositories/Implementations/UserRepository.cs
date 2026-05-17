using KuroOpti.Data;
using KuroOpti.Entities;
using KuroOpti.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KuroOpti.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly KuroOptiDbContext _context;

        public UserRepository(KuroOptiDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user.Id;
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await _context.Users.Where(u => u.Id == id).ExecuteDeleteAsync();
            await _context.SaveChangesAsync();
        }

        public async Task<List<User>> GetAllAsync(int page, int itemsPerPage)
        {
            return await _context
                .Users.Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync();
        }
    }
}
