using ChatApp.Application.Services.Contracts;
using ChatApp.Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace ChatApp.Presentation.Hubs
{
    public class ChatHub : Hub
    {
        #region Fields
        private readonly ICurrentUserService _currentUserService;
        private readonly IOnlineUserService _onlineUserService;
        private readonly IUserService _userService;
        private readonly IChatService _chatService;
        #endregion

        #region Constructors
        public ChatHub(ICurrentUserService currentUserService,
            IOnlineUserService onlineUserService,
            IUserService userService,
            IChatService chatService)
        {
            _currentUserService = currentUserService;
            _onlineUserService = onlineUserService;
            _userService = userService;
            _chatService = chatService;
        }
        #endregion

        #region Functions
        public override async Task OnConnectedAsync()
        {
            var currentUserId = _currentUserService.GetUserId();
            var connectionId = Context.ConnectionId;

            _onlineUserService.UserConnected(currentUserId, connectionId);

            var chats = await _chatService.GetAllChatsOfUserAsync(currentUserId);
            foreach (var chat in chats)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, chat!.Id.ToString());
                await Clients.Group(chat.Id.ToString()).SendAsync("UserOnline", currentUserId.ToString());
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var currentUserId = _currentUserService.GetUserId();
            var connectionId = Context.ConnectionId;

            _onlineUserService.UserDisconnected(currentUserId, connectionId);
            User? user = null;
            try
            {
                user = await _userService.GetUserByIdAsync(currentUserId);
                if (user != null)
                {
                    user.LastSeenAt = DateTimeOffset.UtcNow.ToLocalTime();
                    await _userService.UpdateUserAsync(user);

                    var chats = await _chatService.GetAllChatsOfUserAsync(currentUserId);
                    if (chats.Any())
                    {
                        foreach (var chat in chats)
                        {
                            await Clients.Group(chat!.Id.ToString()).SendAsync("UserOffline", currentUserId.ToString(), user!.LastSeenAt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warning("Failed in update last seen of {UserName} : {Message}", user!.Name, ex.InnerException?.Message ?? ex.Message);
            }

            await base.OnDisconnectedAsync(exception);
        }
        #endregion
    }
}
