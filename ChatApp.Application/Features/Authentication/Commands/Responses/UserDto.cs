namespace ChatApp.Application.Features.Authentication.Commands.Responses
{
    public record UserDto(
        Guid Id,
        string? Name,
        string? PhoneNumber,
        string? Email,
        string? ProfileImageUrl);
}
