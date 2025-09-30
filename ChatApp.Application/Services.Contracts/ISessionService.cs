using ChatApp.Domain.Entities;

namespace ChatApp.Application.Services.Contracts
{
    public interface ISessionService
    {
        Task<string> CreateSessionAsync(Session session);
        Task<string?> GetKeyHashByUserIdAsync(Guid userId);
        Task<string> RevokeSessionAsync(string keyHash);
    }
}
