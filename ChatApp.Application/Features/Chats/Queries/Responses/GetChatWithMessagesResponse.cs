using ChatApp.Application.Features.MessageStatuses.Queries.Responses;
using ChatApp.Domain.Enums;

namespace ChatApp.Application.Features.Chats.Queries.Responses
{
    public record GetChatWithMessagesResponse(
        Guid ChatId,
        bool IsGroup,
        string? ChatName,
        string? ChatImageUrl,
        IReadOnlyList<MessageReceivedDto?> Messages
    );

    public record MessageReceivedDto(
        Guid MessageId,
        Guid SenderId,
        MessageType Type,
        string? Content,
        string? FilePath,
        int? Duration,
        bool IsEdited,
        DateTimeOffset SentAt,
        List<MessageStatusMiniDto> Statuses
    );
}
