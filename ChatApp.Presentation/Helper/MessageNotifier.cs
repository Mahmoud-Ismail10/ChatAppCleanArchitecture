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
        private readonly IChatMemberService _chatMemberService;
        private readonly IOnlineUserService _onlineUserService;
        private readonly IMessageService _messageService;
        #endregion

        #region Constructors
        public MessageNotifier(IHubContext<ChatHub> hub,
            IMessageStatusService messageStatusService,
            IChatMemberService chatMemberService,
            IOnlineUserService onlineUserService,
            IMessageService messageService)
        {
            _hub = hub;
            _messageStatusService = messageStatusService;
            _chatMemberService = chatMemberService;
            _onlineUserService = onlineUserService;
            _messageService = messageService;
        }
        #endregion

        #region Functions
        public async Task NotifyMessageAsync(MessageDto message)
        {
            // Get active users in the chat
            var activeUserIds = await _chatMemberService.GetActiveUsersAsync(message.ChatId);
            var onlineUserIds = _onlineUserService.GetOnlineUsersAsync(activeUserIds);

            await _hub.Clients.Clients(onlineUserIds).SendAsync("ReceiveMessage", message);
        }

        public async Task NotifyUpdatedMessageAsync(MessageDto message)
        {
            // Get active users in the chat
            var activeUserIds = await _chatMemberService.GetActiveUsersAsync(message.ChatId);
            var onlineUserIds = _onlineUserService.GetOnlineUsersAsync(activeUserIds);

            await _hub.Clients.Clients(onlineUserIds).SendAsync("UpdatedMessage", message);
        }

        public async Task NotifyUnreadIncrementAsync(Guid chatId)
        {
            var activeUserIds = await _chatMemberService.GetActiveUsersAsync(chatId);
            var onlineUserIds = _onlineUserService.GetOnlineUsersAsync(activeUserIds);

            await _hub.Clients.Clients(onlineUserIds).SendAsync("UnreadCountIncremented", chatId.ToString());
        }

        public async Task NotifyChatMembersUpdatedAsync(List<string> onlineUserIds, ChatMemberUpdatedDto updatedDto)
        {
            await _hub.Clients.Clients(onlineUserIds).SendAsync("ChatMembersUpdated", updatedDto);
        }

        public async Task NotifyDeletedMessageAsync(Guid chatId, Guid messageId)
        {
            var activeUserIds = await _chatMemberService.GetActiveUsersAsync(chatId);
            var onlineUserIds = _onlineUserService.GetOnlineUsersAsync(activeUserIds);

            await _hub.Clients.Clients(onlineUserIds).SendAsync("MessageDeleted", messageId.ToString());
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
