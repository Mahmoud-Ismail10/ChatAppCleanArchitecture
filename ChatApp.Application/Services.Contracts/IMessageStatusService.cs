using ChatApp.Application.Features.MessageStatuses.Queries.Responses;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Services.Contracts
{
    public interface IMessageStatusService
    {
        Task<List<MessageStatus>> CreateMessageStatusesAsync(Guid chatId, Guid currentUserId, Guid messageId);
        Task<List<MessageStatusDto>> GetMessageStatusesAsync(Guid messageId);
        Task<MessageStatusDto> MarkAsDeliveredAsync(Guid messageId, Guid userId);
        Task<MessageStatusDto> MarkAsPlayedAsync(Guid messageId, Guid userId);
        Task<MessageStatusDto> MarkAsReadAsync(Guid messageId, Guid userId);
    }
}
