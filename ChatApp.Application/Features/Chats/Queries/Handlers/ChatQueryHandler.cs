using ChatApp.Application.Bases;
using ChatApp.Application.Features.Chats.Queries.Models;
using ChatApp.Application.Features.Chats.Queries.Responses;
using ChatApp.Application.Resources;
using ChatApp.Application.Services.Contracts;
using MediatR;
using Microsoft.Extensions.Localization;

namespace ChatApp.Application.Features.Chats.Queries.Handlers
{
    public class ChatQueryHandler : ApiResponseHandler,
        IRequestHandler<GetChatWithMessagesQuery, ApiResponse<GetChatWithMessagesResponse>>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        private readonly ICurrentUserService _currentUserService;
        private readonly IChatMemberService _chatMemberService;
        private readonly IChatService _chatService;
        private readonly IMessageNotifier _messageNotifier;
        #endregion

        #region Constructors
        public ChatQueryHandler(IStringLocalizer<SharedResources> stringLocalizer,
            IChatService chatService,
            IMessageNotifier messageNotifier,
            IChatMemberService chatMemberService,
            ICurrentUserService currentUserService) : base(stringLocalizer)
        {
            _currentUserService = currentUserService;
            _chatMemberService = chatMemberService;
            _stringLocalizer = stringLocalizer;
            _chatService = chatService;
            _messageNotifier = messageNotifier;
        }
        #endregion

        #region Handle Functions
        public async Task<ApiResponse<GetChatWithMessagesResponse>> Handle(GetChatWithMessagesQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var chat = await _chatService.GetChatWithMessagesAsync(request.ChatId);
            if (chat == null) return NotFound<GetChatWithMessagesResponse>(_stringLocalizer[SharedResourcesKeys.ChatNotFound]);

            var chatMember = await _chatMemberService.GetChatMemberByIdAsync(request.ChatMemberId);
            if (chatMember == null || chatMember.UserId != currentUserId)
                return Unauthorized<GetChatWithMessagesResponse>(_stringLocalizer[SharedResourcesKeys.AccessDenied]);

            string? chatName;
            string? chatImageUrl;

            if (chat.IsGroup)
            {
                chatName = chat.Name;
                chatImageUrl = chat.GroupImageUrl;
            }
            else
            {
                var otherMember = await _chatMemberService.GetAnotherUserInSameChatAsync(currentUserId, chat.Id);
                if (otherMember == null) return NotFound<GetChatWithMessagesResponse>(_stringLocalizer[SharedResourcesKeys.ChatNotFound]);
                chatName = otherMember?.User?.Name;
                chatImageUrl = otherMember?.User?.ProfileImageUrl;
            }

            var messagesQuery = chat.Messages.AsQueryable();

            if (chatMember.DeletedAt.HasValue)
                messagesQuery = messagesQuery.Where(m => m.SentAt > chatMember.DeletedAt.Value).AsQueryable();

            var messages = messagesQuery
                .OrderBy(m => m.SentAt)
                .Select(m => new MessageDto(
                    m.Id,
                    m.SenderId,
                    m.Type,
                    m.Content,
                    m.FilePath,
                    m.Duration,
                    m.SentAt
                )).ToList();

            var response = new GetChatWithMessagesResponse(
                chat.Id,
                chat.IsGroup,
                chatName,
                chatImageUrl,
                messages
            );
            await _chatMemberService.MarkAsReadAsync(request.ChatMemberId);
            await _messageNotifier.NotifyChatReadAsync(chatMember.ChatId, currentUserId);

            return Success(response);
        }
        #endregion
    }
}
