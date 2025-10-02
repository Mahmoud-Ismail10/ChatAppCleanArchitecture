using ChatApp.Application.Bases;
using ChatApp.Application.Features.Users.Queries.Models;
using ChatApp.Application.Features.Users.Queries.Responses;
using ChatApp.Application.Resources;
using ChatApp.Application.Services.Contracts;
using MediatR;
using Microsoft.Extensions.Localization;

namespace ChatApp.Application.Features.Users.Queries.Handlers
{
    public class UserQueryHandler : ApiResponseHandler,
        IRequestHandler<GetUserStatusQuery, ApiResponse<GetUserStatusResponse>>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        private readonly IOnlineUserService _onlineUserService;
        private readonly IUserService _userService;
        #endregion

        #region Constructors
        public UserQueryHandler(IStringLocalizer<SharedResources> stringLocalizer,
            IOnlineUserService onlineUserService,
            IUserService userService) : base(stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
            _onlineUserService = onlineUserService;
            _userService = userService;
        }
        #endregion

        #region Handle Functions
        public async Task<ApiResponse<GetUserStatusResponse>> Handle(GetUserStatusQuery request, CancellationToken cancellationToken)
        {
            var isOnline = _onlineUserService.IsUserOnline(request.UserId);
            var user = await _userService.GetUserByIdAsync(request.UserId);
            if (user == null) return NotFound<GetUserStatusResponse>(_stringLocalizer[SharedResourcesKeys.UserNotFound]);
            var userStatus = new GetUserStatusResponse(isOnline, user!.LastSeenAt);
            return Success(userStatus);
        }
        #endregion
    }
}
