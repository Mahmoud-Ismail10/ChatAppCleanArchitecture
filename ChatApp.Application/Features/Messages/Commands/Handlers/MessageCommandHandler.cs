using ChatApp.Application.Bases;
using ChatApp.Application.Features.Messages.Commands.Models;
using ChatApp.Application.Features.Messages.Commands.Responses;
using ChatApp.Application.Features.MessageStatuses.Queries.Responses;
using ChatApp.Application.Resources;
using ChatApp.Application.Services.Contracts;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Localization;
using Serilog;

namespace ChatApp.Application.Features.Messages.Commands.Handlers
{
    public class MessageCommandHandler : ApiResponseHandler,
        IRequestHandler<SendMessageCommand, ApiResponse<string>>,
        IRequestHandler<DeleteMessageCommand, ApiResponse<string>>,
        IRequestHandler<UpdateMessageCommand, ApiResponse<string>>
    {
        #region Fields
        private readonly IChatService _chatService;
        private readonly IFileService _fileService;
        private readonly IMessageService _messageService;
        private readonly IMessageNotifier _messageNotifier;
        private readonly IChatMemberService _chatMemberService;
        private readonly IOnlineUserService _onlineUserService;
        private readonly ITransactionService _transactionService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMessageStatusService _messageStatusService;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        #endregion

        #region Constructors
        public MessageCommandHandler(
            IChatService chatService,
            IFileService fileService,
            IMessageService messageService,
            IMessageNotifier messageNotifier,
            IChatMemberService chatMemberService,
            IOnlineUserService onlineUserService,
            ITransactionService transactionService,
            ICurrentUserService currentUserService,
            IMessageStatusService MessageStatusService,
            IStringLocalizer<SharedResources> stringLocalizer) : base(stringLocalizer)
        {
            _chatService = chatService;
            _fileService = fileService;
            _messageService = messageService;
            _messageNotifier = messageNotifier;
            _chatMemberService = chatMemberService;
            _onlineUserService = onlineUserService;
            _transactionService = transactionService;
            _currentUserService = currentUserService;
            _messageStatusService = MessageStatusService;
            _stringLocalizer = stringLocalizer;
        }
        #endregion

        #region Handle Functions
        public async Task<ApiResponse<string>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            using var transaction = await _transactionService.BeginTransactionAsync();
            try
            {
                var currentUserId = _currentUserService.GetUserId();
                Chat? chat = null;

                if (request.ReceiverId != null)
                {
                    chat = await _chatService.GetChatBetweenUsersAsync(currentUserId, (Guid)request.ReceiverId);
                    if (chat == null)
                    {
                        chat = new Chat
                        {
                            CreatedAt = DateTimeOffset.UtcNow.ToLocalTime(),
                            ChatMembers = new List<ChatMember>
                            {
                                new ChatMember {
                                    UserId = currentUserId,
                                    Role = Role.Member,
                                    JoinedAt = DateTimeOffset.UtcNow.ToLocalTime(),
                                    Status = MemberStatus.Active },

                                new ChatMember {
                                    UserId = (Guid)request.ReceiverId,
                                    Role = Role.Member,
                                    JoinedAt = DateTimeOffset.UtcNow.ToLocalTime(),
                                    Status = MemberStatus.Active }
                            }
                        };
                        var result = await _chatService.CreateChatAsync(chat);
                        if (result != "Success") return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToCreateChat]);
                    }
                    else if (chat.ChatMembers.Any(m => m.IsDeleted))
                    {
                        var deletedMember = chat.ChatMembers.FirstOrDefault(m => m.IsDeleted);
                        var result = await _chatMemberService.RestoreDeletedChatMembersAsync(deletedMember);
                        if (result != "Success") return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToCreateChat]);
                    }
                }
                else if (request.ChatId != null)
                {
                    chat = await _chatService.GetChatWithMessagesAsync((Guid)request.ChatId);
                    if (chat == null) return NotFound<string>(_stringLocalizer[SharedResourcesKeys.ChatNotFound]);
                    var chatMember = await _chatMemberService.IsMemberOfChatAsync(currentUserId, chat.Id);
                    if (!chatMember) return Unauthorized<string>(_stringLocalizer[SharedResourcesKeys.UnauthorizedToSendMessage]);
                }
                else return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.ReceiverOrChatRequired]);

                string? fileUrl = null;
                MessageType messageType = MessageType.Text;

                if (request.FilePath != null)
                {
                    string? messageError = null;
                    (fileUrl, messageType, messageError) = await _fileService.GetFileAsync(request.FilePath, cancellationToken);
                    if (messageError != null)
                        return messageError switch
                        {
                            "FileIsEmpty" => BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FileIsEmpty]),
                            "InvalidFileType" => BadRequest<string>(_stringLocalizer[SharedResourcesKeys.InvalidFileType]),
                            "FileSizeExceedsLimit" => BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FileSizeExceedsLimit]),
                            _ => BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToUploadFile]),
                        };
                }

                var message = new Message
                {
                    Id = Guid.NewGuid(),
                    ChatId = chat!.Id,
                    SenderId = currentUserId,
                    Content = request.MessageContent,
                    FilePath = fileUrl,
                    Type = messageType,
                    Duration = string.IsNullOrEmpty(request.Duration) ? null : int.Parse(request.Duration),
                    SentAt = DateTimeOffset.UtcNow.ToLocalTime(),
                };

                var messageStatuses = await _messageStatusService.CreateMessageStatusesAsync(chat.Id, currentUserId, message.Id);
                message.MessageStatuses = messageStatuses;

                var activeUserIds = await _chatMemberService.GetActiveUsersAsync(message.ChatId);
                var onlineUserIds = _onlineUserService.GetOnlineUsersAsync(activeUserIds);

                var result3 = await _messageService.AddMessageAsync(message);
                if (result3 == "Success")
                {
                    chat.LastMessage = message;
                    chat.LastMessageId = message.Id;

                    var result2 = await _chatService.UpdateChatAsync(chat);
                    if (result2 != "Success")
                    {
                        await _transactionService.RollBackAsync();
                        return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToUpdateChat]);
                    }

                    var updatedDto = new ChatMemberUpdatedDto
                    (
                        chat.Id.ToString(),
                        message.Content!,
                        message.SentAt,
                        message.Type
                    );

                    // Increment unread message count for the receiver
                    await _messageNotifier.NotifyUnreadIncrementAsync(chat.Id);
                    // Notify chat members about the new message
                    await _messageNotifier.NotifyChatMembersUpdatedAsync(onlineUserIds, updatedDto);
                    // If the receiver is online, mark the message as delivered
                    var MembersIds = await _chatMemberService.GetChatMembersIdsAsync(message.ChatId);

                    foreach (var memberId in MembersIds)
                    {
                        if (memberId == currentUserId) continue;

                        if (_onlineUserService.IsUserOnline(memberId))
                            await _messageNotifier.NotifyMarkAsDeliveredAsync(memberId, message.Id);
                    }

                    var messageMapper = new MessageDto
                    (
                        message.Id.ToString(),
                        message.ChatId,
                        message.SenderId,
                        message.Type,
                        message.Content,
                        message.FilePath,
                        message.Duration,
                        message.SentAt,
                        message.IsEdited,
                        message.IsDeleted,
                        message.MessageStatuses.Select(ms => new MessageStatusMiniDto(
                            ms.UserId,
                            ms.Status)).ToList()
                    );

                    // Broadcast the message to the receiver using SignalR
                    await _messageNotifier.NotifyMessageAsync(messageMapper);

                    await _transactionService.CommitAsync();
                    return Success<string>(_stringLocalizer[SharedResourcesKeys.MessageSentSuccessfully], messageMapper);
                }
                await _transactionService.RollBackAsync();
                return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToSendMessage]);
            }
            catch (Exception ex)
            {
                await _transactionService.RollBackAsync();
                Log.Error(ex, "An error occurred while sending message from user {UserId} to user {ReceiverId}", _currentUserService.GetUserId(), request.ReceiverId);
                return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToSendMessage]);
            }
        }

        public async Task<ApiResponse<string>> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var message = await _messageService.GetMessageByIdAsync(request.MessageId);
            if (message == null)
                return NotFound<string>(_stringLocalizer[SharedResourcesKeys.MessageNotFound]);
            if (message.SenderId != currentUserId)
                return Unauthorized<string>(_stringLocalizer[SharedResourcesKeys.UnauthorizedToDeleteMessage]);
            var result = await _messageService.DeleteMessageAsync(message);
            if (result == "Success")
            {
                await _messageNotifier.NotifyDeletedMessageAsync(message.ChatId, message.Id);
                return Deleted<string>(_stringLocalizer[SharedResourcesKeys.MessageDeletedSuccessfully]);
            }
            else if (result == "ChatNotFound")
                return NotFound<string>(_stringLocalizer[SharedResourcesKeys.ChatNotFound]);
            else
                return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToDeleteMessage]);
        }

        public async Task<ApiResponse<string>> Handle(UpdateMessageCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var message = await _messageService.GetMessageByIdAsync(request.MessageId);
            if (message == null)
                return NotFound<string>(_stringLocalizer[SharedResourcesKeys.MessageNotFound]);

            if (message.SenderId != currentUserId)
                return Unauthorized<string>(_stringLocalizer[SharedResourcesKeys.UnauthorizedToUpdateMessage]);

            if (message.Content == request.NewContent)
                return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.NoChangesDetected]);

            var elapsed = DateTime.UtcNow - message.SentAt;
            if (elapsed > TimeSpan.FromMinutes(15))
                return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.EditTimeExpired]);

            message.Content = request.NewContent;
            message.IsEdited = true;

            var result = await _messageService.UpdateMessageAsync(message);
            if (result == "Success")
            {
                var messageMapper = new MessageDto
                (
                    message.Id.ToString(),
                    message.ChatId,
                    message.SenderId,
                    message.Type,
                    message.Content,
                    message.FilePath,
                    message.Duration,
                    message.SentAt,
                    message.IsEdited,
                    message.IsDeleted,
                    message.MessageStatuses.Select(ms => new MessageStatusMiniDto(
                        ms.UserId,
                        ms.Status)).ToList()
                );

                var updatedDto = new ChatMemberUpdatedDto
                 (
                    message.ChatId.ToString(),
                    message.Content,
                    message.SentAt,
                    message.Type
                 );
                var chatMembersIds = new List<string> { currentUserId.ToString() };

                // Notify the chat members about the updated message
                await _messageNotifier.NotifyUpdatedMessageAsync(messageMapper);
                // Notify chat members about the updated message
                await _messageNotifier.NotifyChatMembersUpdatedAsync(chatMembersIds, updatedDto);
                // If the receiver is online, mark the message as delivered
                var MembersIds = await _chatMemberService.GetChatMembersIdsAsync(message.ChatId);

                foreach (var memberId in MembersIds)
                {
                    if (memberId == currentUserId) continue;

                    if (_onlineUserService.IsUserOnline(memberId))
                        await _messageNotifier.NotifyMarkAsDeliveredAsync(memberId, message.Id);
                }

                return Edit<string>(_stringLocalizer[SharedResourcesKeys.MessageUpdatedSuccessfully], messageMapper);
            }
            return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToUpdateMessage]);
        }
        #endregion
    }
}
