using ChatApp.Application.Features.Messages.Commands.Responses;
using ChatApp.Application.Services.Contracts;
using ChatApp.Presentation.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Presentation.Helper
{
    public class MessageNotifier : IMessageNotifier
    {
        #region Fields
        private readonly IHubContext<ChatHub> _hub;
        private readonly IMessageStatusService _messageStatusService;
        private readonly IMessageService _messageService;
        #endregion

        #region Constructors
        public MessageNotifier(IHubContext<ChatHub> hub,
            IMessageStatusService messageStatusService,
            IMessageService messageService)
        {
            _hub = hub;
            _messageStatusService = messageStatusService;
            _messageService = messageService;
        }
        #endregion

        #region Functions
        public async Task NotifyMessageAsync(ReceiveMessageDto message)
        {
            await _hub.Clients.Group(message.ChatId.ToString()).SendAsync("ReceiveMessage", message);
        }

        public async Task NotifyUpdatedMessageAsync(MessageDto message)
        {
            await _hub.Clients.Group(message.ChatId.ToString()).SendAsync("UpdatedMessage", message);
        }

        public async Task NotifyUnreadIncrementAsync(Guid chatId)
        {
            await _hub.Clients.Group(chatId.ToString())
                .SendAsync("UnreadCountIncremented", new { ChatId = chatId.ToString() });
        }

        public async Task NotifyChatMembersUpdatedAsync(List<string> chatMembersIds, ChatMemberUpdatedDto updatedDto)
        {
            await _hub.Clients.Users(chatMembersIds).SendAsync("ChatMembersUpdated", updatedDto);
        }

        public async Task NotifyDeletedMessageAsync(Guid chatId, Guid messageId)
        {
            await _hub.Clients.Group(chatId.ToString())
                .SendAsync("MessageDeleted", new { MessageId = messageId.ToString() });
        }

        // Invoked when a user opens an application
        public async Task NotifyMarkAsDeliveredAsync(Guid receiveUserId, Guid messageId)
        {
            var statusDto = await _messageStatusService.MarkAsDeliveredAsync(messageId, receiveUserId);
            var senderId = await _messageService.GetSenderIdAsync(messageId);

            await _hub.Clients.User(senderId.ToString())
                .SendAsync("MessageDelivered", messageId.ToString(), statusDto);
        }

        // Invoked when a user opens an chat
        public async Task NotifyMarkAsReadAsync(Guid receiveUserId, Guid messageId)
        {
            var statusDto = await _messageStatusService.MarkAsReadAsync(messageId, receiveUserId);
            var senderId = await _messageService.GetSenderIdAsync(messageId);

            await _hub.Clients.User(senderId.ToString())
                .SendAsync("MessageRead", messageId.ToString(), statusDto);
        }

        // Invoked when a user plays a voice message
        public async Task NotifyMarkAsPlayedAsync(Guid receiveUserId, Guid messageId)
        {
            var statusDto = await _messageStatusService.MarkAsPlayedAsync(messageId, receiveUserId);
            var senderId = await _messageService.GetSenderIdAsync(messageId);

            await _hub.Clients.User(senderId.ToString())
                .SendAsync("MessagePlayed", messageId.ToString(), statusDto);
        }

        // Invoked when message statuses are updated in bulk (e.g., multiple messages marked as read)
        public async Task NotifyMessageStatusesUpdatedAsync(Guid messageId)
        {
            var statuses = await _messageStatusService.GetMessageStatusesAsync(messageId);
            var senderId = await _messageService.GetSenderIdAsync(messageId);

            await _hub.Clients.User(senderId.ToString())
                .SendAsync("MessageStatusesUpdated", messageId.ToString(), statuses);
        }
        #endregion
    }
}
