using ChatApp.Application.Bases;
using MediatR;

namespace ChatApp.Application.Features.ChatsMember.Commands.Models
{
    public record DeleteGroupCommand(Guid ChatMemberId) : IRequest<ApiResponse<string>>;
}
