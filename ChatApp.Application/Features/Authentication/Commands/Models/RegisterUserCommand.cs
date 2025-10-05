using ChatApp.Application.Bases;
using MediatR;

namespace ChatApp.Application.Features.Authentication.Commands.Models
{
    public record RegisterUserCommand(
        string? Name,
        string? PhoneNumber,
        string? Email) : IRequest<ApiResponse<string>>;
}
