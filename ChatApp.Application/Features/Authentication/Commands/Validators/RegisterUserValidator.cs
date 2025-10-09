using ChatApp.Application.Features.Authentication.Commands.Models;
using ChatApp.Application.Resources;
using ChatApp.Application.Services.Contracts;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace ChatApp.Application.Features.Authentication.Commands.Validatos
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        private readonly IUserService _userService;
        #endregion

        #region Constructors
        public RegisterUserValidator(IStringLocalizer<SharedResources> stringLocalizer, IUserService userService)
        {
            _stringLocalizer = stringLocalizer;
            _userService = userService;
            ApplyValidationRoles();
        }
        #endregion

        #region Handle Functions
        public void ApplyValidationRoles()
        {
            RuleFor(x => x.Name)
            .NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
            .NotNull().WithMessage(_stringLocalizer[SharedResourcesKeys.Required])
            .MaximumLength(30).WithMessage(_stringLocalizer[SharedResourcesKeys.MaxLengthIs30]);

            RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
            .NotNull().WithMessage(_stringLocalizer[SharedResourcesKeys.Required])
            .Matches(@"^\d{10,15}$").WithMessage(_stringLocalizer[SharedResourcesKeys.PhoneNumberFormat])
            .MustAsync(async (phoneNumber, cancellationToken) =>
            {
                var formattedPhone = "+" + phoneNumber;
                return await _userService.IsPhoneUniqueAsync(formattedPhone, cancellationToken);
            }).WithMessage(_stringLocalizer[SharedResourcesKeys.PhoneNumberAlreadyExists]);

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage(_stringLocalizer[SharedResourcesKeys.InvalidEmailFormat])
                .MaximumLength(50).WithMessage(_stringLocalizer[SharedResourcesKeys.MaxLengthIs50]);
        }
        #endregion
    }
}

