using ChatApp.Application.Resources;
using ChatApp.Domain.Helpers;
using ChatApp.Domain.Repositories.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ChatApp.Presentation.Security
{
    public class SessionKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        private readonly ISessionRepository _sessions;

        public SessionKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            IStringLocalizer<SharedResources> stringLocalizer,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            ISessionRepository sessions)
            : base(options, logger, encoder, clock)
        {
            _stringLocalizer = stringLocalizer;
            _sessions = sessions;
        }

        #region AuthenticateUserFromHeaderOrQuery
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string? rawKey = null;

            if (Request.Headers.TryGetValue("Authorization", out var header))
            {
                var val = header.ToString();
                if (val.StartsWith("Key ", StringComparison.OrdinalIgnoreCase))
                    rawKey = val.Substring("Key ".Length).Trim();
            }

            if (string.IsNullOrEmpty(rawKey))
                rawKey = Uri.UnescapeDataString(Request.Query["rawKey"].ToString()).Trim().Replace(" ", "+");

            if (string.IsNullOrEmpty(rawKey))
                return AuthenticateResult.Fail(_stringLocalizer[SharedResourcesKeys.MissingAuthorizationHeader]);

            var keyHash = SessionKeyHelper.ComputeSha256Hex(rawKey);
            var session = await _sessions.GetByKeyHashAsync(keyHash);

            if (session == null || session.Revoked)
                return AuthenticateResult.Fail(_stringLocalizer[SharedResourcesKeys.InvalidOrRevokedSessionKey]);

            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, session.UserId.ToString()) };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
        #endregion

        // Override HandleChallengeAsync 401
        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 401;
            Response.ContentType = "application/json";

            var failMessage = (Context.Items["AuthFailureMessage"] as string) ?? _stringLocalizer[SharedResourcesKeys.UnAuthorized];
            var json = System.Text.Json.JsonSerializer.Serialize(new { message = failMessage });

            return Response.WriteAsync(json);
        }

        // Override HandleForbiddenAsync 403
        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 403;
            Response.ContentType = "application/json";

            var json = System.Text.Json.JsonSerializer.Serialize(new { message = _stringLocalizer[SharedResourcesKeys.AccessDenied] });
            return Response.WriteAsync(json);
        }
    }
}
