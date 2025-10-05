using ChatApp.Application.Bases;
using ChatApp.Application.Features.Authentication.Commands.Models;
using ChatApp.Application.Features.Authentication.Commands.Responses;
using ChatApp.Application.Resources;
using ChatApp.Application.Services.Contracts;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Helpers;
using MediatR;
using Microsoft.Extensions.Localization;

namespace ChatApp.Application.Features.Authentication.Commands.Handlers
{
    public class AuthenticationCommandHandler : ApiResponseHandler,
        IRequestHandler<SendOtpCommand, ApiResponse<string>>,
        IRequestHandler<VerifyOtpCommand, ApiResponse<string>>,
        IRequestHandler<RegisterUserCommand, ApiResponse<string>>,
        IRequestHandler<CreateSessionCommand, ApiResponse<string>>,
        IRequestHandler<LogoutCommand, ApiResponse<string>>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        private readonly IOtpService _otpService;
        private readonly ISmsService _smsService;
        private readonly IUserService _userService;
        private readonly ISessionService _sessionService;
        private readonly ICurrentUserService _currentUserService;
        #endregion

        #region Constructors
        public AuthenticationCommandHandler(IStringLocalizer<SharedResources> stringLocalizer,
            IOtpService otpService,
            ISmsService smsService,
            IUserService userService,
            ISessionService sessionService,
            ICurrentUserService currentUserService) : base(stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
            _otpService = otpService;
            _smsService = smsService;
            _userService = userService;
            _sessionService = sessionService;
            _currentUserService = currentUserService;
        }
        #endregion

        #region Handle Functions
        public async Task<ApiResponse<string>> Handle(SendOtpCommand request, CancellationToken cancellationToken)
        {
            var formattedPhone = "+" + request.PhoneNumber;
            var otp = await _otpService.GenerateOtpAsync(formattedPhone);
            var message = _stringLocalizer[SharedResourcesKeys.YourOTPCodeIs] + otp;
            var result = await _smsService.SendSmsAsync(formattedPhone, message);
            if (result == "Success")
                return Success<string>(_stringLocalizer[SharedResourcesKeys.OtpSentSuccessfully], request.PhoneNumber);
            return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToSendOtp]);
        }

        public async Task<ApiResponse<string>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            var formattedPhone = "+" + request.PhoneNumber;
            var isValid = await _otpService.VerifyOtpAsync(formattedPhone, request.Otp);
            if (isValid)
            {
                var userExists = await _userService.GetUserByPhoneNumberAsync(formattedPhone);
                if (userExists != null)
                {
                    var userMapper = new UserDto
                    (
                        userExists.Id,
                        userExists.Name,
                        userExists.PhoneNumber,
                        userExists.Email,
                        userExists.ProfileImageUrl
                    );
                    return Success<string>(_stringLocalizer[SharedResourcesKeys.OtpVerifiedSuccessfully], userMapper);
                }
                return Success<string>(_stringLocalizer[SharedResourcesKeys.OtpVerifiedSuccessfully], request.PhoneNumber);
            }
            return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.InvalidOtp]);
        }

        public async Task<ApiResponse<string>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var formattedPhone = "+" + request.PhoneNumber;
            var existingUser = await _userService.GetUserByPhoneNumberAsync(formattedPhone);
            if (existingUser == null)
            {
                var user = new User();
                user.Id = Guid.NewGuid();
                user.Name = request.Name;
                user.PhoneNumber = formattedPhone;
                user.Email = request.Email;
                user.CreatedAt = DateTimeOffset.UtcNow.ToLocalTime();
                user.LastSeenAt = DateTimeOffset.UtcNow.ToLocalTime();

                var result = await _userService.AddUserAsync(user);
                return result switch
                {
                    "Success" => Success<string>(_stringLocalizer[SharedResourcesKeys.UserRegisteredSuccessfully], user.Id),
                    "FailedToUploadImage" => BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToUploadImage]),
                    _ => BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToRegisterUser])
                };
            }
            existingUser.Name = request.Name ?? existingUser.Name;
            existingUser.Email = request.Email;
            existingUser.LastSeenAt = DateTimeOffset.UtcNow.ToLocalTime();

            var updateResult = await _userService.UpdateUserAsync(existingUser);
            return updateResult switch
            {
                "Success" => Success<string>(_stringLocalizer[SharedResourcesKeys.LoginSuccessfully], existingUser.Id),
                "FailedToUploadImage" => BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToUploadImage]),
                _ => BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToRegisterUser])
            };
        }

        public async Task<ApiResponse<string>> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
        {
            var rawKey = SessionKeyHelper.GenerateKeyBase64();
            var keyHash = SessionKeyHelper.ComputeSha256Hex(rawKey).ToLowerInvariant();

            var session = new Session();
            session.Id = Guid.NewGuid();
            session.UserId = request.UserId;
            session.KeyHash = keyHash;
            session.Revoked = false;
            session.CreatedAt = DateTimeOffset.UtcNow.ToLocalTime();

            var result = await _sessionService.CreateSessionAsync(session);
            if (result == "Success")
                return Success<string>(_stringLocalizer[SharedResourcesKeys.SessionCreatedSuccessfully], rawKey);
            return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToCreateSession]);
        }

        public async Task<ApiResponse<string>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var keyHash = await _sessionService.GetKeyHashByUserIdAsync(currentUserId);
            if (keyHash == null)
                return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.NoActiveSessionFound]);

            var result = await _sessionService.RevokeSessionAsync(keyHash);
            if (result == "Success")
                return Success<string>(_stringLocalizer[SharedResourcesKeys.LoggedOutSuccessfully]);
            return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToLogout]);
        }
        #endregion
    }
}
