using ChatApp.Application.Bases;
using MediatR;

namespace ChatApp.Application.Features.Authentication.Commands.Models
{
    public record CreateSessionCommand(Guid UserId) : IRequest<ApiResponse<string>>;
}
