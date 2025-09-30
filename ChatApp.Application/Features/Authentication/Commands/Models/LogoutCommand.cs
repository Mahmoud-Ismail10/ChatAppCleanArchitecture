using ChatApp.Application.Bases;
using MediatR;

namespace ChatApp.Application.Features.Authentication.Commands.Models
{
    public record LogoutCommand : IRequest<ApiResponse<string>>;
}
