using ChatApp.Application.Bases;
using ChatApp.Application.Features.Contacts.Commands.Models;
using ChatApp.Application.Resources;
using ChatApp.Application.Services.Contracts;
using ChatApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace ChatApp.Application.Features.Contacts.Commands.Handlers
{
    public class ContactCommandHandler : ApiResponseHandler,
        IRequestHandler<AddToContactsByPhoneNumberCommand, ApiResponse<string>>,
        IRequestHandler<RemoveFromContactsCommand, ApiResponse<string>>
    {
        #region Fields
        private readonly IUserService _userService;
        private readonly IContactService _contactService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        #endregion

        #region Constructors
        public ContactCommandHandler(
            IUserService userService,
            IContactService contactService,
            ICurrentUserService currentUserService,
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<SharedResources> stringLocalizer) : base(stringLocalizer)
        {
            _userService = userService;
            _contactService = contactService;
            _currentUserService = currentUserService;
            _httpContextAccessor = httpContextAccessor;
            _stringLocalizer = stringLocalizer;
        }
        #endregion

        #region Handle Functions
        public async Task<ApiResponse<string>> Handle(AddToContactsByPhoneNumberCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.GetUserId();

            var formattedPhone = "+" + request.PhoneNumber;
            var existingUser = await _userService.GetUserByPhoneNumberAsync(formattedPhone);
            if (existingUser == null)
                return NotFound<string>(_stringLocalizer[SharedResourcesKeys.UserNotFound]);

            var existingContact = await _contactService.GetContactAsync(userId, existingUser.Id);
            if (existingContact != null)
                return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.ContactAlreadyExists]);

            var contact = new Contact
            {
                UserId = userId,
                ContactUserId = existingUser.Id,
                CreatedAt = DateTimeOffset.UtcNow.ToLocalTime()
            };
            var result = await _contactService.AddToContactAsync(contact);
            if (result == "Success")
                return Success<string>(_stringLocalizer[SharedResourcesKeys.ContactCreatedSuccessfully], existingUser.Id);
            return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToCreateContact]);
        }

        public async Task<ApiResponse<string>> Handle(RemoveFromContactsCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var result = await _contactService.RemoveFromContactsAsync(currentUserId, request.contactId);
            return result switch
            {
                "NotFound" => NotFound<string>(_stringLocalizer[SharedResourcesKeys.ContactNotFound]),
                "Unauthorized" => Unauthorized<string>(_stringLocalizer[SharedResourcesKeys.UnauthorizedToRemoveContact]),
                "Failed" => BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToRemoveContact]),
                _ => Success<string>(_stringLocalizer[SharedResourcesKeys.ContactRemovedSuccessfully])
            };
        }
        #endregion
    }
}
