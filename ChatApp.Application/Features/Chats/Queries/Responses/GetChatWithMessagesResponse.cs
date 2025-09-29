using ChatApp.Domain.Enums;

namespace ChatApp.Application.Features.Chats.Queries.Responses
{
    public record GetChatWithMessagesResponse(
        Guid ChatId,
        bool IsGroup,
        string? ChatName,
        string? ChatImageUrl,
        IReadOnlyList<MessageDto?> Messages
    );

    public record MessageDto(
        Guid MessageId,
        Guid SenderId,
        MessageType Type,
        string? Content,
        DateTimeOffset SentAt
    );
}
