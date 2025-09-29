using ChatApp.Application.Bases;
using ChatApp.Application.Features.ChatsMember.Queries.Models;
using ChatApp.Application.Features.ChatsMember.Queries.Responses;
using ChatApp.Application.Resources;
using ChatApp.Application.Services.Contracts;
using MediatR;
using Microsoft.Extensions.Localization;

namespace ChatApp.Application.Features.ChatsMember.Queries.Handlers
{
    public class ChatMemberCommandHandler : ApiResponseHandler,
        IRequestHandler<GetAllChatsMemberQuery, ApiResponse<List<GetAllChatsMemberResponse>>>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        private readonly ICurrentUserService _currentUserService;
        private readonly IChatMemberService _chatMemberService;
        private readonly IUserService _userService;
        #endregion

        #region Constructors
        public ChatMemberCommandHandler(IStringLocalizer<SharedResources> stringLocalizer,
            ICurrentUserService currentUserService,
            IChatMemberService chatMemberService,
            IUserService userService) : base(stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
            _currentUserService = currentUserService;
            _chatMemberService = chatMemberService;
            _userService = userService;
        }
        #endregion

        #region Handle Functions
        public async Task<ApiResponse<List<GetAllChatsMemberResponse>>> Handle(GetAllChatsMemberQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var chatMembers = await _chatMemberService.GetAllChatsMemberAsync(currentUserId);
            if (chatMembers == null || !chatMembers.Any())
                return NotFound<List<GetAllChatsMemberResponse>>(_stringLocalizer[SharedResourcesKeys.NoChatsFound]);
            var responseTasks = chatMembers.Select(async m =>
            {
                string? chatName;
                string? chatImageUrl;

                if (m!.Chat!.IsGroup)
                {
                    chatName = m.Chat.Name;
                    chatImageUrl = m.Chat.GroupImageUrl;
                }
                else
                {
                    var otherMember = await _chatMemberService.GetAnotherUserInSameChatAsync(currentUserId, m.Chat.Id);
                    chatName = otherMember?.User?.Name;
                    chatImageUrl = otherMember?.User?.ProfileImageUrl;
                }
                return new GetAllChatsMemberResponse(
                    m.Chat.Id,
                    m.Chat.IsGroup,
                    chatName,
                    chatImageUrl,
                    m.Chat.LastMessage?.Type,
                    m.Chat.LastMessage?.Content,
                    m.Chat.LastMessage?.SentAt);
            }).ToList();
            var responses = (await Task.WhenAll(responseTasks)).ToList();
            return Success(responses);
        }
        #endregion
    }
}
