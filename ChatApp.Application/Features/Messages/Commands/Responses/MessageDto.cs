using ChatApp.Domain.Enums;

namespace ChatApp.Application.Features.Messages.Commands.Responses
{
    public record MessageDto(
        string Id,
        Guid ChatId,
        Guid SenderId,
        MessageType Type,
        string? Content,
        string? FilePath,
        int? Duration,
        DateTimeOffset SentAt,
        bool IsEdited,
        bool IsDeleted);
}
