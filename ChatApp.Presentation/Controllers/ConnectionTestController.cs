using ChatApp.Application.Bases;
using ChatApp.Application.Services.Contracts;
using ChatApp.Domain.AppMetaData;
using ChatApp.Presentation.Base;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ChatApp.Presentation.Controllers
{
    public class ConnectionTestController : AppControllerBase
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IOnlineUserService _onlineUserService;
        private readonly IChatService _chatService;
        private readonly IMessageNotifier _messageNotifier;
        private readonly IUserService _userService;

        public ConnectionTestController(
            ICurrentUserService currentUserService,
            IOnlineUserService onlineUserService,
            IChatService chatService,
            IMessageNotifier messageNotifier,
            IUserService userService)
        {
            _currentUserService = currentUserService;
            _onlineUserService = onlineUserService;
            _chatService = chatService;
            _messageNotifier = messageNotifier;
            _userService = userService;
        }

        [HttpPost(Router.ConnectionTest.Connect)]
        public IActionResult SimulateConnect()
        {
            var currentUserId = _currentUserService.GetUserId();
            var connectionId = Guid.NewGuid().ToString();
            _onlineUserService.UserConnected(currentUserId, connectionId);

            var response = new
            {
                Message = $"User {currentUserId} connected.",
                ConnectionId = connectionId
            };

            var apiResponse = new ApiResponse<object>
            {
                StatusCode = HttpStatusCode.OK,
                Succeeded = true,
                Message = "تمت العملية بنجاح",
                Data = response
            };

            return NewResult(apiResponse);
        }

        [HttpPost(Router.ConnectionTest.Disconnect)]
        public async Task<IActionResult> SimulateDisconnect()
        {
            var currentUserId = _currentUserService.GetUserId();
            _onlineUserService.UserDisconnected(currentUserId, "fake-connection-id");

            var user = await _userService.GetUserByIdAsync(currentUserId);
            if (user != null)
            {
                user.LastSeenAt = DateTimeOffset.UtcNow;
                await _userService.UpdateUserAsync(user);
            }

            var response = $"User {currentUserId} disconnected.";
            var apiResponse = new ApiResponse<object>
            {
                StatusCode = HttpStatusCode.OK,
                Succeeded = true,
                Message = "تمت العملية بنجاح",
                Data = response
            };

            return NewResult(apiResponse);
        }
    }
}
