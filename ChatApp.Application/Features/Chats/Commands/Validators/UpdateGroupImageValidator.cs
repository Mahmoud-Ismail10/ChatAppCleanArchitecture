using ChatApp.Application.Features.Chats.Commands.Models;
using ChatApp.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace ChatApp.Application.Features.Chats.Commands.Validators
{
    public class UpdateGroupImageValidator : AbstractValidator<UpdateGroupImageCommand>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        #endregion

        #region Constructors
        public UpdateGroupImageValidator(IStringLocalizer<SharedResources> stringLocalizer)
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

            RuleFor(x => x.GroupImageUrl)
                .NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
                .NotNull().WithMessage(_stringLocalizer[SharedResourcesKeys.Required]);
        }
        #endregion
    }
}
