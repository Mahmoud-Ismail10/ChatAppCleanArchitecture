using ChatApp.Domain.Enums;

namespace ChatApp.Application.Features.ChatsMember.Queries.Responses
{
    public record GetAllChatsMemberResponse(
        Guid? ChatOtherMemberId,
        Guid ChatMemberId,
        Guid ChatId,
        bool IsGroup,
        bool IsPinned,
        bool? IsOnline,
        string? ChatName,
        string? ChatImageUrl,
        MessageType? MessageType,
        string? LastMessage,
        DateTimeOffset? LastMessageSendAt,
        int UnreadCount);

}
