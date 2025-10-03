using System.Security.Claims;

namespace ChatApp.Application.Services.Contracts
{
    public interface ICurrentUserService
    {
        Guid GetUserId();
        ClaimsPrincipal IsAuthenticated();
    }
}
