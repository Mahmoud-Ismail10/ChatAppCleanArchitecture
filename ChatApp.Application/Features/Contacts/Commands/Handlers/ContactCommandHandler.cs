using ChatApp.Application.Bases;
using ChatApp.Application.Features.Contacts.Commands.Models;
using ChatApp.Application.Resources;
using ChatApp.Application.Services.Contracts;
using ChatApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System.Security.Claims;

namespace ChatApp.Application.Features.Contacts.Commands.Handlers
{
    public class ContactCommandHandler : ApiResponseHandler,
        IRequestHandler<AddToContactsByPhoneNumberCommand, ApiResponse<string>>
    {
        #region Fields
        private readonly IUserService _userService;
        private readonly IContactService _contactService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        #endregion

        #region Constructors
        public ContactCommandHandler(
            IUserService userService,
            IContactService contactService,
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<SharedResources> stringLocalizer) : base(stringLocalizer)
        {
            _userService = userService;
            _contactService = contactService;
            _httpContextAccessor = httpContextAccessor;
            _stringLocalizer = stringLocalizer;
        }
        #endregion

        #region Handle Functions
        public async Task<ApiResponse<string>> Handle(AddToContactsByPhoneNumberCommand request, CancellationToken cancellationToken)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
                return Unauthorized<string>(_stringLocalizer[SharedResourcesKeys.UnAuthorized]);
            var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var existingUser = await _userService.GetUserByPhoneNumberAsync(request.PhoneNumber);
            if (existingUser == null)
                return NotFound<string>(_stringLocalizer[SharedResourcesKeys.UserNotFound]);

            var existingContact = await _contactService.GetContactAsync(Guid.Parse(userId!), existingUser.Id);
            if (existingContact != null)
                return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.ContactAlreadyExists]);

            var contact = new Contact
            {
                UserId = Guid.Parse(userId!),
                ContactUserId = existingUser.Id,
                CreatedAt = DateTimeOffset.UtcNow.ToLocalTime()
            };
            var result = await _contactService.AddToContactAsync(contact);
            if (result == "Success")
                return Success<string>(_stringLocalizer[SharedResourcesKeys.ContactCreatedSuccessfully], existingUser.Id);
            return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToCreateContact]);
        }
        #endregion
    }
}
