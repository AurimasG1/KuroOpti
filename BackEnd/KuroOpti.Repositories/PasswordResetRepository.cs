using KuroOpti.Data;
using KuroOpti.Entities;
using KuroOpti.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KuroOpti.Repositories
{
    public class PasswordResetRepository : IPasswordResetRepository
    {
        private readonly KuroOptiDbContext db;

        public PasswordResetRepository(KuroOptiDbContext db)
        {
            this.db = db;
        }

        public async Task AddAsync(PasswordResetToken token)
        {
            await db.PasswordResetTokens.AddAsync(token);
            await db.SaveChangesAsync();
        }

        public async Task<PasswordResetToken?> GetValidTokenAsync(string token)
        {
            return await db
                .PasswordResetTokens.Include(x => x.User)
                .FirstOrDefaultAsync(x =>
                    x.Token == token && !x.Used && x.ExpiresAt > DateTime.UtcNow
                );
        }

        public async Task MarkAsUsedAsync(PasswordResetToken token)
        {
            token.Used = true;
            db.PasswordResetTokens.Update(token);
            await db.SaveChangesAsync();
        }
    }
}
