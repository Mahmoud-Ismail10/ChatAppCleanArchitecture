using ChatApp.Application.Features.Messages.Commands.Models;
using ChatApp.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace ChatApp.Application.Features.Messages.Commands.Validators
{
    public class SendMessageToContactValidator : AbstractValidator<SendMessageToContactCommand>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        #endregion

        #region Constructors
        public SendMessageToContactValidator(IStringLocalizer<SharedResources> stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
            ApplyValidationRoles();
        }
        #endregion

        #region Handle Functions
        public void ApplyValidationRoles()
        {
            RuleFor(x => x.ReceiverId)
                .NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
                .NotNull().WithMessage(_stringLocalizer[SharedResourcesKeys.Required]);

            When(x => x.FilePath == null, () =>
            {
                RuleFor(x => x.MessageContent)
                    .NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
                    .NotNull().WithMessage(_stringLocalizer[SharedResourcesKeys.Required]);
            });
        }
        #endregion
    }
}

