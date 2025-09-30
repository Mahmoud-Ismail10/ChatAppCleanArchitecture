using ChatApp.Domain.Entities;

namespace ChatApp.Domain.Repositories.Contracts
{
    public interface ISessionRepository
    {
        Task AddAsync(Session session);
        Task<Session?> GetByKeyHashAsync(string keyHash);
        Task<string?> GetKeyHashByUserIdAsync(Guid userId);
        Task RevokeAsync(string keyHash);
    }
}
