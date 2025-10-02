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
        IRequestHandler<GetAllContactsQuery, ApiResponse<List<GetAllContactsResponse>>>,
        IRequestHandler<ViewContactQuery, ApiResponse<ViewContactResponse>>
    {
        #region Fields
        private readonly IUserService _userService;
        private readonly IContactService _contactService;
        private readonly IChatMemberService _chatMemberService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        #endregion

        #region Constructors
        public ContactQueryHandler(
            IUserService userService,
            IContactService contactService,
            IChatMemberService chatMemberService,
            ICurrentUserService currentUserService,
            IStringLocalizer<SharedResources> stringLocalizer) : base(stringLocalizer)
        {
            _userService = userService;
            _contactService = contactService;
            _chatMemberService = chatMemberService;
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

        public async Task<ApiResponse<ViewContactResponse>> Handle(ViewContactQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var chatMember = await _chatMemberService.GetChatMemberByIdAsync(request.ChatOtherMemberId);
            if (chatMember == null) return NotFound<ViewContactResponse>(_stringLocalizer[SharedResourcesKeys.ChatNotFound]);

            var user = await _userService.GetUserByIdAsync(chatMember.UserId);
            if (user == null) return NotFound<ViewContactResponse>(_stringLocalizer[SharedResourcesKeys.UserNotFound]);

            var response = new ViewContactResponse(
                chatMember.ChatId,
                chatMember.Id,
                user.Id,
                user.Name!,
                user.PhoneNumber!,
                user.ProfileImageUrl,
                user.LastSeenAt
            );
            return Success(response);
        }
        #endregion
    }
}
