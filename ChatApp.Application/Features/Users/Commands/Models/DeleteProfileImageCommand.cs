using ChatApp.Application.Bases;
using MediatR;

namespace ChatApp.Application.Features.Users.Commands.Models
{
    public record DeleteProfileImageCommand() : IRequest<ApiResponse<string>>;
}
