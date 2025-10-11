using ChatApp.Application.Bases;
using MediatR;

namespace ChatApp.Application.Features.ChatsMember.Commands.Models
{
    public record MakeAsAdminOrUnadminCommand(Guid ChatMemberId) : IRequest<ApiResponse<string>>;
}
