using ChatApp.Domain.Helpers;
using ChatApp.Domain.Repositories.Contracts;
using System.Security.Claims;

namespace ChatApp.Presentation.Middlewares
{
    public class SessionKeyAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionKeyAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ISessionRepository sessions)
        {
            if (context.Request.Headers.TryGetValue("Authorization", out var header))
            {
                var val = header.ToString();
                if (val.StartsWith("Key ", StringComparison.OrdinalIgnoreCase))
                {
                    var rawKey = val.Substring("Key ".Length).Trim();
                    var keyHash = SessionKeyHelper.ComputeSha256Hex(rawKey);
                    var session = await sessions.GetByKeyHashAsync(keyHash);
                    if (session != null && !session.Revoked)
                    {
                        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, session.UserId.ToString()) };
                        var identity = new ClaimsIdentity(claims, "SessionKey");
                        context.User = new ClaimsPrincipal(identity);
                    }
                }
            }

            await _next(context);
        }
    }
}
