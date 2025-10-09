using ChatApp.Application.Features.Contacts.Commands.Models;
using ChatApp.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace ChatApp.Application.Features.Contacts.Commands.Validators
{
    public class RemoveFromContactsValidator : AbstractValidator<AddToContactsByPhoneNumberCommand>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        #endregion

        #region Constructors
        public RemoveFromContactsValidator(IStringLocalizer<SharedResources> stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
            ApplyValidationRoles();
        }
        #endregion

        #region Functions
        public void ApplyValidationRoles()
        {
            RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
            .NotNull().WithMessage(_stringLocalizer[SharedResourcesKeys.Required])
            .Matches(@"^\d{10,15}$").WithMessage(_stringLocalizer[SharedResourcesKeys.PhoneNumberFormat]);
        }
        #endregion
    }
}

