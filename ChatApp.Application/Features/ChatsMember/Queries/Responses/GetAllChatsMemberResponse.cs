using ChatApp.Domain.Enums;

namespace ChatApp.Application.Features.ChatsMember.Queries.Responses
{
    public record GetAllChatsMemberResponse(
        Guid ChatMemberId,
        Guid ChatId,
        bool IsGroup,
        bool? IsOnline,
        string? ChatName,
        string? ChatImageUrl,
        MessageType? MessageType,
        string? LastMessage,
        DateTimeOffset? LastMessageSendAt,
        int UnreadCount);

}
