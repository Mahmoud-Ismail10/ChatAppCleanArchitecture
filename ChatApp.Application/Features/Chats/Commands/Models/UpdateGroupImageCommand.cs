using ChatApp.Application.Bases;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ChatApp.Application.Features.Chats.Commands.Models
{
    public record UpdateGroupImageCommand(Guid ChatId, IFormFile GroupImageUrl) : IRequest<ApiResponse<string>>;
}
