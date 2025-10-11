using ChatApp.Application.Bases;
using MediatR;

namespace ChatApp.Application.Features.Chats.Commands.Models
{
    public record CreateGroupCommand(
        string Name,
        string? Description,
        List<Guid> UsersId) : IRequest<ApiResponse<string>>;
}
