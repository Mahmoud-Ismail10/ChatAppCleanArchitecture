using ChatApp.Application.Features.Chats.Queries.Responses;
using ChatApp.Application.Features.Messages.Commands.Responses;

namespace ChatApp.Application.Services.Contracts
{
    public interface IMessageNotifier
    {
        Task NotifyChatMembersUpdatedAsync(List<string> chatMembersIds, ChatMemberUpdatedDto updatedDto);
        Task NotifyChatReadAsync(ChatReadDto readDto);
        Task NotifyDeletedMessageAsync(Guid chatId, Guid messageId);
        Task NotifyMarkAsReadAsync(Guid currentUserId, Guid messageId);
        Task NotifyMessageAsync(MessageDto message);
        Task NotifyUnreadIncrementAsync(Guid chatId);
        Task NotifyUpdatedMessageAsync(MessageDto message);
    }
}
