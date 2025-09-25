using ChatApp.Application.Bases;
using ChatApp.Application.Features.Chats.Queries.Responses;
using MediatR;

namespace ChatApp.Application.Features.Chats.Queries.Models
{
    public record GetChatWithMessagesQuery(Guid ChatId) : IRequest<ApiResponse<GetChatWithMessagesResponse>>;
}
