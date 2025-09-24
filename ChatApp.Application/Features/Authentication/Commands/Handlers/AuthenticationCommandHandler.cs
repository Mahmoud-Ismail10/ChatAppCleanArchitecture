using ChatApp.Application.Bases;
using ChatApp.Application.Features.Authentication.Commands.Models;
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
        IRequestHandler<CreateSessionCommand, ApiResponse<string>>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        private readonly IOtpService _otpService;
        private readonly ISmsService _smsService;
        private readonly IUserService _userService;
        private readonly ISessionService _sessionService;
        #endregion

        #region Constructors
        public AuthenticationCommandHandler(IStringLocalizer<SharedResources> stringLocalizer,
            IOtpService otpService,
            ISmsService smsService,
            IUserService userService,
            ISessionService sessionService) : base(stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
            _otpService = otpService;
            _smsService = smsService;
            _userService = userService;
            _sessionService = sessionService;
        }
        #endregion

        #region Handle Functions
        public async Task<ApiResponse<string>> Handle(SendOtpCommand request, CancellationToken cancellationToken)
        {
            var otp = await _otpService.GenerateOtpAsync(request.PhoneNumber);
            var message = _stringLocalizer[SharedResourcesKeys.YourOTPCodeIs] + otp;
            var result = await _smsService.SendSmsAsync(request.PhoneNumber, message);
            if (result == "Success")
                return Success<string>(_stringLocalizer[SharedResourcesKeys.OtpSentSuccessfully], request.PhoneNumber);
            return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToSendOtp]);
        }

        public async Task<ApiResponse<string>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            var isValid = await _otpService.VerifyOtpAsync(request.PhoneNumber, request.Otp);
            if (isValid)
                return Success<string>(_stringLocalizer[SharedResourcesKeys.OtpVerifiedSuccessfully], request.PhoneNumber);
            return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.InvalidOtp]);
        }

        public async Task<ApiResponse<string>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User();
            user.Id = Guid.NewGuid();
            user.PhoneNumber = request.PhoneNumber;
            user.CreatedAt = DateTimeOffset.UtcNow.ToLocalTime();
            user.LastSeenAt = DateTimeOffset.UtcNow.ToLocalTime();

            var result = await _userService.AddUserAsync(user);
            if (result == "Success")
                return Success<string>(_stringLocalizer[SharedResourcesKeys.UserRegisteredSuccessfully], user.Id);
            return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToRegisterUser]);
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
        #endregion

    }
}
