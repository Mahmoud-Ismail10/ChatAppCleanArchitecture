namespace ChatApp.Application.Features.Users.Queries.Responses
{
    public record GetUserStatusResponse(bool IsOnline, DateTimeOffset? LastSeenAt);
}
