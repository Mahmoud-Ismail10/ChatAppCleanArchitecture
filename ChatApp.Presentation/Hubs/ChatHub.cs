using ChatApp.Application.Services.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Presentation.Hubs
{
    public class ChatHub : Hub
    {
        #region Fields
        private readonly ICurrentUserService _currentUserService;
        private readonly IChatService _chatService;
        #endregion

        #region Constructors
        public ChatHub(ICurrentUserService currentUserService, IChatService chatService)
        {
            _currentUserService = currentUserService;
            _chatService = chatService;
        }
        #endregion

        #region Functions
        public override async Task OnConnectedAsync()
        {
            var currentUserId = _currentUserService.GetUserId();

            var chats = await _chatService.GetAllChatsOfUserAsync(currentUserId);
            foreach (var chat in chats)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, chat!.Id.ToString());
            }

            await base.OnConnectedAsync();
        }
        #endregion
    }
}
