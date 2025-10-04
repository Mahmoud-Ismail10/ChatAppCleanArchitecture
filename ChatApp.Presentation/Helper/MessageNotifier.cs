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
        public async Task NotifyMessageAsync(SendMessageDto message)
        {
            await _hub.Clients.Group(message.ChatId.ToString()).SendAsync("ReceiveMessage", message);
        }

        public async Task NotifyUnreadIncrementAsync(Guid chatId)
        {
            await _hub.Clients.Group(chatId.ToString()).SendAsync("UnreadCountIncremented", new { ChatId = chatId });
        }

        public async Task NotifyChatReadAsync(Guid chatId, Guid userId)
        {
            await _hub.Clients.Group(chatId.ToString()).SendAsync("ChatRead", new { ChatId = chatId, UserId = userId });
        }

        #endregion
    }
}
