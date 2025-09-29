using ChatApp.Domain.Enums;

namespace ChatApp.Application.Features.ChatsMember.Queries.Responses
{
    public record GetAllChatsMemberResponse(
        Guid ChatId,
        bool IsGroup,
        string? ChatName,
        string? ChatImageUrl,
        MessageType? MessageType,
        string? LastMessage,
        DateTimeOffset? LastMessageSendAt);

}
