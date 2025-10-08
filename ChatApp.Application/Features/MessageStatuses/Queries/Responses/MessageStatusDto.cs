using ChatApp.Domain.Enums;

namespace ChatApp.Application.Features.MessageStatuses.Queries.Responses
{
    public record MessageStatusDto(
        Guid UserId,
        MessageState Status,
        DateTimeOffset? DeliveredAt,
        DateTimeOffset? ReadAt,
        DateTimeOffset? PlayedAt,
        string? UserName,
        string? UserProfileImageUrl);
}
