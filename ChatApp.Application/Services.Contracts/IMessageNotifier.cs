using ChatApp.Application.Features.Messages.Commands.Responses;

namespace ChatApp.Application.Services.Contracts
{
    public interface IMessageNotifier
    {
        Task NotifyChatMembersUpdatedAsync(List<string> chatMembersIds, ChatMemberUpdatedDto updatedDto);
        Task NotifyDeletedMessageAsync(Guid chatId, Guid messageId);
        Task NotifyMarkAsDeliveredAsync(Guid receiveUserId, Guid messageId);
        Task NotifyMarkAsReadAsync(Guid receiveUserId, Guid messageId);
        Task NotifyMessageAsync(MessageDto message);
        Task NotifyUnreadIncrementAsync(Guid chatId);
        Task NotifyUpdatedMessageAsync(MessageDto message);
    }
}
