using ChatApp.Application.Bases;
using ChatApp.Application.Features.Chats.Queries.Models;
using ChatApp.Application.Features.Chats.Queries.Responses;
using ChatApp.Application.Features.MessageStatuses.Queries.Responses;
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

            // If the user has deleted the chat, show only messages sent after deletion
            if (chatMember.DeletedAt.HasValue)
                messagesQuery = messagesQuery.Where(m => m.SentAt > chatMember.DeletedAt.Value);
            // If the user has left the chat, show only messages sent before leaving
            else if (chatMember.LeftAt != default && chatMember.JoinedAt != default)
            {
                messagesQuery = messagesQuery.Where(m =>
                    m.SentAt <= chatMember.LeftAt || m.SentAt >= chatMember.JoinedAt);
            }

            var messages = messagesQuery
                .OrderBy(m => m.SentAt)
                .Select(m => new MessageReceivedDto(
                    m.Id,
                    m.SenderId,
                    m.Type,
                    m.Content,
                    m.FilePath,
                    m.Duration,
                    m.IsEdited,
                    m.SentAt,
                    m.MessageStatuses.Select(s => new MessageStatusMiniDto(
                        s.UserId,
                        s.Status)).ToList()
                )).ToList();

            var response = new GetChatWithMessagesResponse(
                chat.Id,
                chat.IsGroup,
                chatName,
                chatImageUrl,
                messages
            );

            var readStatuses = await _chatMemberService.MarkAllAsReadAsync(request.ChatMemberId);

            // Notify senders of messages in the chat that messages have been read from current user
            foreach (var status in readStatuses)
            {
                await _messageNotifier.NotifyMarkAsReadAsync(currentUserId, status!.MessageId);
            }

            return Success(response);
        }
        #endregion
    }
}
