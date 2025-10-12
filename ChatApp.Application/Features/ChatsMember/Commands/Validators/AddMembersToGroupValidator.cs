using ChatApp.Application.Features.ChatsMember.Commands.Models;
using ChatApp.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace ChatApp.Application.Features.ChatsMember.Commands.Validators
{
    public class AddMembersToGroupValidator : AbstractValidator<AddMembersToGroupCommand>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        #endregion

        #region Constructors
        public AddMembersToGroupValidator(IStringLocalizer<SharedResources> stringLocalizer)
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

            RuleFor(x => x.UserIds)
                .NotNull().WithMessage(_stringLocalizer[SharedResourcesKeys.Required])
                .Must(u => u != null && u.Any()).WithMessage(_stringLocalizer[SharedResourcesKeys.AtLeastSelectOneUser]);
        }
        #endregion
    }
}
