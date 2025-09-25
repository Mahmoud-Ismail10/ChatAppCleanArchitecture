using ChatApp.Domain.Helpers;
using ChatApp.Domain.Repositories.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ChatApp.Presentation.Security
{
    public class SessionKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ISessionRepository _sessions;

        public SessionKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            ISessionRepository sessions)
            : base(options, logger, encoder, clock)
        {
            _sessions = sessions;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var header))
                return AuthenticateResult.Fail("Missing Authorization header");

            var val = header.ToString();
            if (!val.StartsWith("Key ", StringComparison.OrdinalIgnoreCase))
                return AuthenticateResult.Fail("Invalid scheme");

            var rawKey = val.Substring("Key ".Length).Trim();
            var keyHash = SessionKeyHelper.ComputeSha256Hex(rawKey);
            var session = await _sessions.GetByKeyHashAsync(keyHash);

            if (session == null || session.Revoked)
                return AuthenticateResult.Fail("Invalid or revoked session key");

            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, session.UserId.ToString()) };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }

        // Override HandleChallengeAsync 401
        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 401;
            Response.ContentType = "application/json";

            var failMessage = (Context.Items["AuthFailureMessage"] as string) ?? "Unauthorized access";
            var json = System.Text.Json.JsonSerializer.Serialize(new { message = failMessage });

            return Response.WriteAsync(json);
        }

        // Override HandleForbiddenAsync 403
        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 403;
            Response.ContentType = "application/json";

            var json = System.Text.Json.JsonSerializer.Serialize(new { message = "Forbidden" });
            return Response.WriteAsync(json);
        }
    }
}
