using ChatApp.Domain.Enums;

namespace ChatApp.Application.Features.MessageStatuses.Queries.Responses
{
    public record MessageStatusMiniDto(Guid UserId, MessageState Status);
}
