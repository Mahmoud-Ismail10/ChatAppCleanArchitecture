using ChatApp.Application.Features.Users.Commands.Models;
using ChatApp.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace ChatApp.Application.Features.Users.Commands.Validators
{
    public class UpdateProfileValidator : AbstractValidator<UpdateProfileCommand>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        #endregion

        #region Constructors
        public UpdateProfileValidator(IStringLocalizer<SharedResources> stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
            ApplyValidationRoles();
        }
        #endregion

        #region Functions
        public void ApplyValidationRoles()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
                .NotNull().WithMessage(_stringLocalizer[SharedResourcesKeys.Required])
                .MaximumLength(30).WithMessage(_stringLocalizer[SharedResourcesKeys.MaxLengthIs30]);

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage(_stringLocalizer[SharedResourcesKeys.InvalidEmailFormat])
                .MaximumLength(50).WithMessage(_stringLocalizer[SharedResourcesKeys.MaxLengthIs50]);
        }
        #endregion
    }
}