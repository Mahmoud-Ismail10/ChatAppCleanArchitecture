using ChatApp.Application.Bases;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ChatApp.Application.Features.Messages.Commands.Models
{
    public record SendMessageCommand(
        Guid? ReceiverId,
        Guid? ChatId,
        string? MessageContent,
        IFormFile? FilePath,
        string? Duration) : IRequest<ApiResponse<string>>;
}
