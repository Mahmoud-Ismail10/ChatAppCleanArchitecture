using ChatApp.Application.Bases;
using MediatR;

namespace ChatApp.Application.Features.Chats.Commands.Models
{
    public record UpdateGroupCommand(Guid ChatId, string Name, string? Description) : IRequest<ApiResponse<string>>;
}
