using ChatApp.Application.Bases;
using MediatR;

namespace ChatApp.Application.Features.Messages.Commands.Models
{
    public record UpdateMessageCommand(Guid MessageId, string NewContent) : IRequest<ApiResponse<string>>;
}
