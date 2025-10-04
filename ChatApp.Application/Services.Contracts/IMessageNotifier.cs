using ChatApp.Application.Features.Messages.Commands.Responses;

namespace ChatApp.Application.Services.Contracts
{
    public interface IMessageNotifier
    {
        Task NotifyChatReadAsync(Guid chatId, Guid userId);
        Task NotifyMessageAsync(SendMessageDto message);
        Task NotifyUnreadIncrementAsync(Guid chatId);
    }
}
