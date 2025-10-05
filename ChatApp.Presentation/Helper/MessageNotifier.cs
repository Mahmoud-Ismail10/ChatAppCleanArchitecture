using ChatApp.Application.Features.Chats.Queries.Responses;
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
        #endregion

        #region Constructors
        public MessageNotifier(IHubContext<ChatHub> hub)
        {
            _hub = hub;
        }
        #endregion

        #region Functions
        public async Task NotifyMessageAsync(MessageDto message)
        {
            await _hub.Clients.Group(message.ChatId.ToString()).SendAsync("ReceiveMessage", message);
        }

        public async Task NotifyUpdatedMessageAsync(MessageDto message)
        {
            await _hub.Clients.Group(message.ChatId.ToString()).SendAsync("UpdatedMessage", message);
        }

        public async Task NotifyUnreadIncrementAsync(Guid chatId)
        {
            await _hub.Clients.Group(chatId.ToString()).SendAsync("UnreadCountIncremented", new { ChatId = chatId.ToString() });
        }

        public async Task NotifyChatReadAsync(ChatReadDto readDto)
        {
            await _hub.Clients.Group(readDto.ChatId).SendAsync("ChatRead", readDto);
        }

        public async Task NotifyChatMembersUpdatedAsync(List<string> chatMembersIds, ChatMemberUpdatedDto updatedDto)
        {
            await _hub.Clients.Users(chatMembersIds).SendAsync("ChatMembersUpdated", updatedDto);
        }

        public async Task NotifyDeletedMessageAsync(Guid chatId, Guid messageId)
        {
            await _hub.Clients.Group(chatId.ToString()).SendAsync("MessageDeleted", new { MessageId = messageId.ToString() });
        }
        #endregion
    }
}
