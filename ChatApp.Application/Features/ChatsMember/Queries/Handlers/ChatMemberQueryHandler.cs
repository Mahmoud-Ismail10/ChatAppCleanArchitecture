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
            var chatMembers =
                await _chatMemberService.GetAllChatsMemberAsync(currentUserId);
            if (chatMembers == null || !chatMembers.Any())
                return NotFound<List<GetAllChatsMemberResponse>>(_stringLocalizer[SharedResourcesKeys.NoChatsFound]);

            var responses = new List<GetAllChatsMemberResponse>();
            foreach (var cm in chatMembers.Where(cm => !cm!.IsDeleted))
            {
                string? chatName;
                string? chatImageUrl;
                bool? isOnline = null;
                Guid? chatOtherMemberId = null;
                Guid userId = Guid.Empty;
                var unreadCount = await _messageService.CountAsync(
                    m => cm!.LastReadMessageAt == null || m.SentAt > cm.LastReadMessageAt.Value);

                if (cm!.Chat!.IsGroup)
                {
                    chatName = cm.Chat.Name;
                    chatImageUrl = cm.Chat.GroupImageUrl;
                }
                else
                {
                    var otherMember = await _chatMemberService.GetAnotherUserInSameChatAsync(currentUserId, cm.Chat.Id);
                    chatOtherMemberId = otherMember!.Id;
                    isOnline = _onlineUserService.IsUserOnline(otherMember!.UserId);
                    chatName = otherMember?.User?.Name;
                    chatImageUrl = otherMember?.User?.ProfileImageUrl;
                    userId = otherMember!.UserId;
                }

                responses.Add(new GetAllChatsMemberResponse(
                    chatOtherMemberId,
                    cm.Id,
                    cm.Chat.Id,
                    cm.Chat.IsGroup,
                    isOnline,
                    chatName,
                    chatImageUrl,
                    cm.Chat.LastMessage?.Type,
                    cm.Chat.LastMessage?.Content,
                    cm.Chat.LastMessage?.SentAt,
                    unreadCount));
            }

            var orderedResponses = responses
                .OrderBy(r => r.LastMessageSendAt)
                .ToList();

            if (!orderedResponses.Any())
                return NotFound<List<GetAllChatsMemberResponse>>(_stringLocalizer[SharedResourcesKeys.NoChatsFound]);
            return Success(responses);
        }
        #endregion
    }
}
