using ChatApp.Application.Bases;
using ChatApp.Application.Features.Contacts.Queries.Models;
using ChatApp.Application.Features.Contacts.Queries.Responses;
using ChatApp.Application.Resources;
using ChatApp.Application.Services.Contracts;
using MediatR;
using Microsoft.Extensions.Localization;

namespace ChatApp.Application.Features.Contacts.Queries.Handlers
{
    public class ContactQueryHandler : ApiResponseHandler,
        IRequestHandler<GetAllContactsQuery, ApiResponse<List<GetAllContactsResponse>>>
    {
        #region Fields
        private readonly IContactService _contactService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        #endregion

        #region Constructors
        public ContactQueryHandler(
            IContactService contactService,
            ICurrentUserService currentUserService,
            IStringLocalizer<SharedResources> stringLocalizer) : base(stringLocalizer)
        {
            _contactService = contactService;
            _currentUserService = currentUserService;
            _stringLocalizer = stringLocalizer;
        }
        #endregion

        #region Handle Functions
        public async Task<ApiResponse<List<GetAllContactsResponse>>> Handle(GetAllContactsQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var contacts = await _contactService.GetAllContactsAsync(currentUserId);
            if (contacts == null || !contacts.Any())
                return NotFound<List<GetAllContactsResponse>>(_stringLocalizer[SharedResourcesKeys.NoContactsFound]);

            var response = contacts.Select(c => new GetAllContactsResponse(
                c!.Id,
                c.ContactUserId,
                c.ContactUser!.Name!,
                c.ContactUser!.PhoneNumber!,
                c.ContactUser?.ProfileImageUrl
            )).ToList();
            return Success(response);
        }
        #endregion
    }
}
