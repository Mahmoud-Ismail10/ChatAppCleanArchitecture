using ChatApp.Application.Services.Contracts;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Repositories.Contracts;
using Serilog;

namespace ChatApp.Infrastructure.Services
{
    public class SessionService : ISessionService
    {
        #region Fields
        //private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISessionRepository _sessionRepository;
        #endregion

        #region Constructors
        public SessionService(/*IHttpContextAccessor httpContextAccessor,*/
            ISessionRepository sessionRepository)
        {
            //_httpContextAccessor = httpContextAccessor;
            _sessionRepository = sessionRepository;
        }
        #endregion

        #region Functions
        //public string? GetUserId()
        //{
        //    return _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
        //}

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
        #endregion
    }
}
