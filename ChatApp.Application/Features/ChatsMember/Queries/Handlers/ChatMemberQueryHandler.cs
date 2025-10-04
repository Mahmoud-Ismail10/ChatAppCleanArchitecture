using ChatApp.Application.Bases;
using ChatApp.Application.Features.ChatsMember.Queries.Models;
using ChatApp.Application.Features.ChatsMember.Queries.Responses;
using ChatApp.Application.Resources;
using ChatApp.Application.Services.Contracts;
using MediatR;
using Microsoft.Extensions.Localization;

namespace ChatApp.Application.Features.ChatsMember.Queries.Handlers
{
    public class ChatMemberQueryHandler : ApiResponseHandler,
        IRequestHandler<GetAllChatsMemberQuery, ApiResponse<List<GetAllChatsMemberResponse>>>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        private readonly ICurrentUserService _currentUserService;
        private readonly IOnlineUserService _onlineUserService;
        private readonly IChatMemberService _chatMemberService;
        private readonly IMessageService _messageService;
        #endregion

        #region Constructors
        public ChatMemberQueryHandler(IStringLocalizer<SharedResources> stringLocalizer,
            ICurrentUserService currentUserService,
            IOnlineUserService onlineUserService,
            IChatMemberService chatMemberService,
            IMessageService messageService) : base(stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
            _currentUserService = currentUserService;
            _onlineUserService = onlineUserService;
            _chatMemberService = chatMemberService;
            _messageService = messageService;
        }
        #endregion

        #region Handle Functions
        public async Task<ApiResponse<List<GetAllChatsMemberResponse>>> Handle(GetAllChatsMemberQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var chatMembers = await _chatMemberService.GetAllChatsMemberAsync(currentUserId);
            if (chatMembers == null || !chatMembers.Any())
                return NotFound<List<GetAllChatsMemberResponse>>(_stringLocalizer[SharedResourcesKeys.NoChatsFound]);

            var responses = chatMembers.Select(chat =>
            {
                bool? isOnline = null;

                if (!chat.IsGroup && chat.ReceiverUserId.HasValue)
                    isOnline = _onlineUserService.IsUserOnline(chat.ReceiverUserId.Value);

                return chat with { IsOnline = isOnline };
            }).OrderByDescending(r => r.LastMessageSentAt).ToList();

            return Success(responses);
        }
        #endregion
    }
}
