using ChatApp.Application.Services.Contracts;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Repositories.Contracts;
using Serilog;

namespace ChatApp.Infrastructure.Services
{
    public class SessionService : ISessionService
    {
        #region Fields
        private readonly ISessionRepository _sessionRepository;
        #endregion

        #region Constructors
        public SessionService(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }
        #endregion

        #region Functions
        public async Task<string> CreateSessionAsync(Session session)
        {
            try
            {
                await _sessionRepository.AddAsync(session);
                return "Success";
            }
            catch (Exception ex)
            {
                Log.Error("Error adding session : {ErrorMessage}", ex.InnerException?.Message ?? ex.Message);
                return "Failed";
            }
        }

        public async Task<string> RevokeSessionAsync(string keyHash)
        {
            try
            {
                await _sessionRepository.RevokeAsync(keyHash);
                return "Success";
            }
            catch (Exception ex)
            {
                Log.Error("Error in revoked session : {ErrorMessage}", ex.InnerException?.Message ?? ex.Message);
                return "Failed";
            }
        }

        public async Task<string?> GetKeyHashByUserIdAsync(Guid userId)
        {
            return await _sessionRepository.GetKeyHashByUserIdAsync(userId);
        }
        #endregion
    }
}
