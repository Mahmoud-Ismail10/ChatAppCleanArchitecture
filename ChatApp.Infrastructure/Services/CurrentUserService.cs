using ChatApp.Application.Resources;
using ChatApp.Application.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System.Security.Authentication;
using System.Security.Claims;

namespace ChatApp.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        #region Fields
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        #endregion

        #region Constructors
        public CurrentUserService(IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<SharedResources> stringLocalizer)
        {
            _httpContextAccessor = httpContextAccessor;
            _stringLocalizer = stringLocalizer;
        }
        #endregion

        #region Handle Functions
        public ClaimsPrincipal IsAuthenticated()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
                throw new AuthenticationException(_stringLocalizer[SharedResourcesKeys.UnAuthorized]);
            return user;
        }

        public Guid GetUserId()
        {
            return Guid.Parse(IsAuthenticated().FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        }
        #endregion
    }
}
