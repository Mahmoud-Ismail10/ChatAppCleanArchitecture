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
        public async Task NotifyMessageToContactAsync(SendMessageDto message)
        {
            await _hub.Clients.Group(message.ChatId.ToString()).SendAsync("ReceiveMessage", message);
        }
        #endregion
    }
}
