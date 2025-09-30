using ChatApp.Application.Bases;
using ChatApp.Application.Features.Messages.Commands.Models;
using ChatApp.Application.Features.Messages.Commands.Responses;
using ChatApp.Application.Resources;
using ChatApp.Application.Services.Contracts;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Localization;

namespace ChatApp.Application.Features.Messages.Commands.Handlers
{
    public class MessageCommandHandler : ApiResponseHandler,
        IRequestHandler<SendMessageToContactCommand, ApiResponse<string>>
    {
        #region Fields
        private readonly IChatService _chatService;
        private readonly IMessageService _messageService;
        private readonly IMessageNotifier _messageNotifier;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        #endregion

        #region Constructors
        public MessageCommandHandler(
            IChatService chatService,
            IMessageService messageService,
            IMessageNotifier messageNotifier,
            ICurrentUserService currentUserService,
            IStringLocalizer<SharedResources> stringLocalizer) : base(stringLocalizer)
        {
            _chatService = chatService;
            _messageService = messageService;
            _messageNotifier = messageNotifier;
            _currentUserService = currentUserService;
            _stringLocalizer = stringLocalizer;
        }
        #endregion

        #region Handle Functions
        public async Task<ApiResponse<string>> Handle(SendMessageToContactCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var chat = await _chatService.GetChatBetweenUsersAsync(currentUserId, request.ReceiverId);
            if (chat == null)
            {
                chat = new Chat
                {
                    CreatedAt = DateTimeOffset.UtcNow.ToLocalTime(),
                    ChatMembers = new List<ChatMember>
                    {
                        new ChatMember { UserId = currentUserId, Role = Role.Member, JoinedAt = DateTimeOffset.UtcNow.ToLocalTime() },
                        new ChatMember { UserId = request.ReceiverId, Role = Role.Member, JoinedAt = DateTimeOffset.UtcNow.ToLocalTime() }
                    }
                };
                var result1 = await _chatService.CreateChatAsync(chat);
                if (result1 != "Success") return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToCreateChat]);
            }
            string? fileUrl = null;
            MessageType messageType = MessageType.Text;

            if (request.FilePath != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(request.FilePath.FileName);
                var filePath = Path.Combine("wwwroot/Files", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.FilePath.CopyToAsync(stream, cancellationToken);
                }

                fileUrl = "/Files/" + fileName;
                messageType = request.FilePath == null ? MessageType.Text
                    : request.FilePath.ContentType.ToLower() switch
                    {
                        var ct when ct.StartsWith("image/") => MessageType.Image,
                        var ct when ct.StartsWith("audio/") => MessageType.Audio,
                        var ct when ct.StartsWith("video/") => MessageType.Video,
                        "application/pdf" => MessageType.PDF,
                        _ => MessageType.Document
                    };
            }

            var message = new Message
            {
                ChatId = chat.Id,
                SenderId = currentUserId,
                Content = request.MessageContent,
                FilePath = fileUrl,
                Type = messageType,
                Duration = request.Duration,
                SentAt = DateTimeOffset.UtcNow.ToLocalTime(),
            };

            var messageMapper = new SendMessageDto
            (
                Guid.NewGuid(),
                message.ChatId,
                message.SenderId,
                message.Type,
                message.Content,
                message.FilePath,
                message.Duration,
                message.SentAt,
                false,
                false
            );

            var result3 = await _messageService.AddMessageAsync(message);
            if (result3 == "Success")
            {
                chat.LastMessage = message;
                chat.LastMessageId = message.Id;
                var result2 = await _chatService.UpdateChatAsync(chat);
                if (result2 != "Success") return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToUpdateChat]);

                // Broadcast the message to the receiver using SignalR
                await _messageNotifier.NotifyMessageToContactAsync(messageMapper);
                // Increment unread message count for the receiver
                await _messageNotifier.NotifyUnreadIncrementAsync(chat.Id);

                return Success<string>(_stringLocalizer[SharedResourcesKeys.MessageSentSuccessfully]);
            }
            return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToSendMessage]);
        }
        #endregion
    }
}
