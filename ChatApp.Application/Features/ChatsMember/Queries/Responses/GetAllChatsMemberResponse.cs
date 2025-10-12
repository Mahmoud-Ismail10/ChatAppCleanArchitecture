using ChatApp.Domain.Enums;

namespace ChatApp.Application.Features.ChatsMember.Queries.Responses
{
    public record GetAllChatsMemberResponse(
        Guid? ChatOtherMemberId,
        Guid? ReceiverUserId,
        Guid ChatMemberId,
        Guid ChatId,
        bool IsGroup,
        bool IsPinned,
        bool? IsOnline,
        string? ChatName,
        string? ChatImageUrl,
        MessageType? LastMessageType,
        string? LastMessageContent,
        DateTimeOffset? LastMessageSentAt,
        MessageState? LastMessageState,
        int UnreadCount);
}
