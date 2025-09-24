using ChatApp.Application.Bases;
using MediatR;

namespace ChatApp.Application.Features.Authentication.Commands.Models
{
    public record RegisterUserCommand(string PhoneNumber) : IRequest<ApiResponse<string>>;
}
