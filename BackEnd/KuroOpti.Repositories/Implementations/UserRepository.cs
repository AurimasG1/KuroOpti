using KuroOpti.Data;
using KuroOpti.Entities;
using KuroOpti.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KuroOpti.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly KuroOptiDbContext db;

        public UserRepository(KuroOptiDbContext db)
        {
            this.db = db;
        }

        public async Task<int> CreateAsync(User user)
        {
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return user.Id;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await db.Users.FindAsync(id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await db.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task UpdateEmailAsync(int userId, string newEmail)
        {
            var user = await db.Users.FindAsync(userId);
            if (user == null)
                return;

            user.Email = newEmail;
            await db.SaveChangesAsync();
        }

        public async Task UpdatePasswordHashAsync(int userId, string newPasswordHash)
        {
            var user = await db.Users.FindAsync(userId);
            if (user == null)
                return;

            user.PasswordHash = newPasswordHash;
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await db.Users.Where(u => u.Id == id).ExecuteDeleteAsync();
            await db.SaveChangesAsync();
        }

        public async Task<List<User>> GetAllAsync(int page, int itemsPerPage)
        {
            return await db.Users.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await db.Users.CountAsync();
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await db.Users.ToListAsync();
        }

        public Task SaveChangesAsync() => db.SaveChangesAsync();
    }
}
