using ChatApp.Application.Bases;
using MediatR;

namespace ChatApp.Application.Features.ChatsMember.Commands.Models
{
    public record LeftGroupCommand(Guid ChatMemberId) : IRequest<ApiResponse<string>>;
}
