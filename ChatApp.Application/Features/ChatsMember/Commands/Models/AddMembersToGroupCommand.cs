using ChatApp.Application.Bases;
using MediatR;

namespace ChatApp.Application.Features.ChatsMember.Commands.Models
{
    public record AddMembersToGroupCommand(Guid ChatId, List<Guid> UserIds) : IRequest<ApiResponse<string>>;
}
