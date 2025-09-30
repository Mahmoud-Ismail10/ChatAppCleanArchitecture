using ChatApp.Domain.Entities;
using ChatApp.Domain.Repositories.Contracts;
using ChatApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly ChatAppDbContext _dbContext;

        public SessionRepository(ChatAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Session?> GetByKeyHashAsync(string keyHash)
        {
            return await _dbContext.Sessions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.KeyHash == keyHash && !s.Revoked);
        }

        public async Task AddAsync(Session session)
        {
            _dbContext.Sessions.Add(session);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RevokeAsync(string keyHash)
        {
            var session = await _dbContext.Sessions.FirstOrDefaultAsync(s => s.KeyHash == keyHash);
            if (session != null)
            {
                session.Revoked = true;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<string?> GetKeyHashByUserIdAsync(Guid userId)
        {
            var session = await _dbContext.Sessions.FirstOrDefaultAsync(s => s.UserId == userId);
            if (session != null) return session.KeyHash;
            return null;
        }
    }
}