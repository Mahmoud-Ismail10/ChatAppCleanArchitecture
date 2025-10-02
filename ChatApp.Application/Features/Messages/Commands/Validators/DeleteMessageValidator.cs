using ChatApp.Application.Features.Messages.Commands.Models;
using ChatApp.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace ChatApp.Application.Features.Messages.Commands.Validators
{
    internal class DeleteMessageValidator : AbstractValidator<DeleteMessageCommand>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        #endregion

        #region Constructors
        public DeleteMessageValidator(IStringLocalizer<SharedResources> stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
            ApplyValidationRoles();
        }
        #endregion

        #region Handle Functions
        public void ApplyValidationRoles()
        {
            RuleFor(x => x.MessageId)
                .NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
                .NotNull().WithMessage(_stringLocalizer[SharedResourcesKeys.Required]);
        }
        #endregion
    }
}

