using ChatApp.Application.Bases;
using MediatR;

namespace ChatApp.Application.Features.Messages.Commands.Models
{
    public record DeleteMessageCommand(Guid MessageId) : IRequest<ApiResponse<string>>;
}
