using ChatApp.Application.Bases;
using ChatApp.Application.Features.Contacts.Queries.Models;
using ChatApp.Application.Features.Contacts.Queries.Responses;
using ChatApp.Application.Resources;
using ChatApp.Application.Services.Contracts;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System.Security.Claims;

namespace ChatApp.Application.Features.Contacts.Queries.Handlers
{
    public class ContactQueryHandler : ApiResponseHandler,
        IRequestHandler<GetAllContactsQuery, ApiResponse<List<GetAllContactsResponse>>>
    {
        #region Fields
        private readonly IContactService _contactService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        #endregion

        #region Constructors
        public ContactQueryHandler(
            IContactService contactService,
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<SharedResources> stringLocalizer) : base(stringLocalizer)
        {
            _contactService = contactService;
            _httpContextAccessor = httpContextAccessor;
            _stringLocalizer = stringLocalizer;
        }
        #endregion

        #region Handle Functions
        public async Task<ApiResponse<List<GetAllContactsResponse>>> Handle(GetAllContactsQuery request, CancellationToken cancellationToken)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
                return Unauthorized<List<GetAllContactsResponse>>(_stringLocalizer[SharedResourcesKeys.UnAuthorized]);
            var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var contacts = await _contactService.GetAllContactsAsync(Guid.Parse(userId!));
            if (contacts == null || !contacts.Any())
                return NotFound<List<GetAllContactsResponse>>(_stringLocalizer[SharedResourcesKeys.NoContactsFound]);

            var response = contacts.Select(c => new GetAllContactsResponse(
                c!.Id,
                c.ContactUser!.Name!,
                c.ContactUser!.PhoneNumber!,
                c.ContactUser?.ProfileImageUrl
            )).ToList();
            return Success(response);
        }
        #endregion
    }
}
