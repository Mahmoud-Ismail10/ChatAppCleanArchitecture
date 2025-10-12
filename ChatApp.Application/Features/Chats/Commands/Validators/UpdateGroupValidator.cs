using ChatApp.Application.Features.Chats.Commands.Models;
using ChatApp.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace ChatApp.Application.Features.Chats.Commands.Validators
{
    public class UpdateGroupValidator : AbstractValidator<UpdateGroupCommand>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        #endregion

        #region Constructors
        public UpdateGroupValidator(IStringLocalizer<SharedResources> stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
            ApplyValidationRoles();
        }
        #endregion

        #region Functions
        public void ApplyValidationRoles()
        {
            RuleFor(x => x.ChatId)
                .NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
                .NotNull().WithMessage(_stringLocalizer[SharedResourcesKeys.Required]);

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
                .NotNull().WithMessage(_stringLocalizer[SharedResourcesKeys.Required])
                .MaximumLength(30).WithMessage(_stringLocalizer[SharedResourcesKeys.MaxLength].Value.Replace("{MaxLength}", "30"));

            RuleFor(x => x.Description)
                .MaximumLength(100).WithMessage(_stringLocalizer[SharedResourcesKeys.MaxLength].Value.Replace("{MaxLength}", "100"))
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
        #endregion
    }
}
