using ChatApp.Domain.Enums;

namespace ChatApp.Application.Features.Chats.Queries.Responses
{
    public record GetChatWithMessagesResponse(
        Guid ChatId,
        bool IsGroup,
        string? Name,
        string? GroupImageUrl,
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
