using ChatApp.Application.Bases;
using MediatR;

namespace ChatApp.Application.Features.ChatsMember.Commands.Models
{
    public record DeleteChatForMeCommand(Guid ChatMemberId) : IRequest<ApiResponse<string>>;
}
