using KuroOpti.Data;
using KuroOpti.Entities;
using KuroOpti.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KuroOpti.Repositories.Implementations
{
    public class PasswordResetRepository : IPasswordResetRepository
    {
        private readonly KuroOptiDbContext db;

        public PasswordResetRepository(KuroOptiDbContext db)
        {
            this.db = db;
        }

        // CREATE TOKEN
        public async Task AddAsync(PasswordResetToken token)
        {
            db.PasswordResetTokens.Add(token);
            await db.SaveChangesAsync();
        }

        // GET VALID TOKEN (not expired, not used)
        public async Task<PasswordResetToken?> GetValidTokenAsync(string token)
        {
            return await db
                .PasswordResetTokens.Include(t => t.User)
                .FirstOrDefaultAsync(t =>
                    t.Token == token && t.ExpiresAt > DateTime.UtcNow && !t.Used
                );
        }

        // MARK TOKEN AS USED
        public async Task MarkAsUsedAsync(PasswordResetToken token)
        {
            token.Used = true;
            await db.SaveChangesAsync();
        }
    }
}
