using ChatApp.Application.Bases;
using MediatR;

namespace ChatApp.Application.Features.Users.Commands.Models
{
    public record UpdateProfileCommand(string? Name, string? Email) : IRequest<ApiResponse<string>>;
}
