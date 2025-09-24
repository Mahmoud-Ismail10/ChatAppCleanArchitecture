using ChatApp.Domain.Entities;

namespace ChatApp.Application.Services.Contracts
{
    public interface ISessionService
    {
        Task<string> CreateSessionAsync(Session session);
    }
}
