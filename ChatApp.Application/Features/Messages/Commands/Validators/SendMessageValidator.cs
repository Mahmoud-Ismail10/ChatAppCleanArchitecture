using ChatApp.Application.Features.Messages.Commands.Models;
using ChatApp.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace ChatApp.Application.Features.Messages.Commands.Validators
{
    public class SendMessageValidator : AbstractValidator<SendMessageCommand>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        #endregion

        #region Constructors
        public SendMessageValidator(IStringLocalizer<SharedResources> stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
            ApplyValidationRoles();
        }
        #endregion

        #region Functions
        public void ApplyValidationRoles()
        {
            RuleFor(x => x).Must(x => (x.ReceiverId.HasValue && !x.ChatId.HasValue) || (!x.ReceiverId.HasValue && x.ChatId.HasValue))
                    .WithMessage(_stringLocalizer[SharedResourcesKeys.ReceiverOrChatRequired]);

            When(x => x.FilePath == null, () =>
            {
                RuleFor(x => x.MessageContent)
                    .NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
                    .NotNull().WithMessage(_stringLocalizer[SharedResourcesKeys.Required])
                    .MaximumLength(1000).WithMessage(_stringLocalizer[SharedResourcesKeys.MaxLength].Value.Replace("{MaxLength}", "1000"));
            });
        }
        #endregion
    }
}

