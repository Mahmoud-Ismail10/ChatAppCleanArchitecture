namespace ChatApp.Application.Features.Users.Queries.Responses
{
    public record GetCurrentUserResponse(
        Guid Id,
        string? Name,
        string? Email,
        string? PhoneNumber,
        DateTimeOffset CreatedAt,
        string? ProfileImageUrl
    );
}
