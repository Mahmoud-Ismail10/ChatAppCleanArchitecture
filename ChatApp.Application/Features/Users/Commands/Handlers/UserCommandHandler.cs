using ChatApp.Application.Bases;
using ChatApp.Application.Features.Users.Commands.Models;
using ChatApp.Application.Resources;
using ChatApp.Application.Services.Contracts;
using MediatR;
using Microsoft.Extensions.Localization;

namespace ChatApp.Application.Features.Users.Commands.Handlers
{
    public class UserCommandHandler : ApiResponseHandler,
        IRequestHandler<UpdateProfileCommand, ApiResponse<string>>,
        IRequestHandler<UpdateProfileImageCommand, ApiResponse<string>>,
        IRequestHandler<DeleteProfileImageCommand, ApiResponse<string>>
    {
        #region Fields
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        #endregion

        #region Constructors
        public UserCommandHandler(
            IUserService userService,
            ICurrentUserService currentUserService,
            IStringLocalizer<SharedResources> stringLocalizer) : base(stringLocalizer)
        {
            _userService = userService;
            _currentUserService = currentUserService;
            _stringLocalizer = stringLocalizer;
        }
        #endregion

        #region Handle Functions
        public async Task<ApiResponse<string>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var user = await _userService.GetUserByIdAsync(currentUserId);
            if (user == null) return NotFound<string>(_stringLocalizer[SharedResourcesKeys.UserNotFound]);

            user.Name = request.Name ?? user.Name;
            user.Email = request.Email;

            var result = await _userService.UpdateUserAsync(user);
            if (result == "Success")
                return Success<string>(_stringLocalizer[SharedResourcesKeys.ProfileUpdatedSuccessfully]);
            return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToUpdateProfile]);
        }

        public async Task<ApiResponse<string>> Handle(UpdateProfileImageCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var user = await _userService.GetUserByIdAsync(currentUserId);
            if (user == null) return NotFound<string>(_stringLocalizer[SharedResourcesKeys.UserNotFound]);

            var result = await _userService.UpdateProfileImageAsync(user, request.ProfileImage);
            return result switch
            {
                "Success" => Success<string>(_stringLocalizer[SharedResourcesKeys.ProfileImageUpdatedSuccessfully]),
                "NoImageProvided" => BadRequest<string>(_stringLocalizer[SharedResourcesKeys.NoImageProvided]),
                _ => BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToUpdateProfileImage])
            };
        }

        public async Task<ApiResponse<string>> Handle(DeleteProfileImageCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var user = await _userService.GetUserByIdAsync(currentUserId);
            if (user == null) return NotFound<string>(_stringLocalizer[SharedResourcesKeys.UserNotFound]);
            if (string.IsNullOrEmpty(user.ProfileImageUrl))
                return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.NoProfileImageToDelete]);
            var result = await _userService.DeleteProfileImageAsync(user);
            if (result == "Success")
                return Success<string>(_stringLocalizer[SharedResourcesKeys.ProfileImageDeletedSuccessfully]);
            return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToDeleteProfileImage]);
        }
        #endregion
    }
}
