using ChatApp.Domain.Enums;

namespace ChatApp.Application.Features.ChatsMember.Queries.Responses
{
    public record GetAllChatsMemberResponse(
        Guid Id,
        bool IsGroup,
        string? Name,
        string? GroupImageUrl,
        MessageType? MessageType,
        string? LastMessage,
        DateTimeOffset? LastMessageSendAt);

}
