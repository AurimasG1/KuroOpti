using KuroOpti.Data;
using KuroOpti.Entities;
using KuroOpti.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KuroOpti.Repositories.Implementations
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly KuroOptiDbContext db;

        public RefreshTokenRepository(KuroOptiDbContext db)
        {
            this.db = db;
        }

        public async Task SaveAsync(RefreshToken token)
        {
            db.RefreshTokens.Add(token);
            await db.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetAsync(string token)
        {
            return await db.RefreshTokens.FirstOrDefaultAsync(t =>
                t.Token == token && !t.IsRevoked
            );
        }

        public async Task RevokeAsync(string token)
        {
            var rt = await db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token);
            if (rt == null)
                return;

            rt.IsRevoked = true;
            await db.SaveChangesAsync();
        }
    }
}
