using ChatApp.Application.Bases;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ChatApp.Application.Features.Messages.Commands.Models
{
    public record SendMessageToContactCommand(
        Guid ReceiverId,
        string? MessageContent,
        IFormFile? FilePath,
        int? Duration) : IRequest<ApiResponse<string>>;
}
