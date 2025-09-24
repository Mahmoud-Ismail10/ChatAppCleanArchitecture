using ChatApp.Application.Features.Authentication.Commands.Models;
using ChatApp.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace ChatApp.Application.Features.Authentication.Commands.Validatos
{
    public class VerifyOtpValidator : AbstractValidator<VerifyOtpCommand>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        #endregion

        #region Constructors
        public VerifyOtpValidator(IStringLocalizer<SharedResources> stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
            ApplyValidationRoles();
        }
        #endregion

        #region Handle Functions
        public void ApplyValidationRoles()
        {
            RuleFor(x => x.Otp)
                .NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
                .NotNull().WithMessage(_stringLocalizer[SharedResourcesKeys.Required])
                .Length(6).WithMessage(_stringLocalizer[SharedResourcesKeys.OtpLengthIs6]);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
                .NotNull().WithMessage(_stringLocalizer[SharedResourcesKeys.Required])
                .Matches(@"^\+\d{10,15}$").WithMessage(_stringLocalizer[SharedResourcesKeys.PhoneNumberFormat]);
        }
        #endregion
    }
}
