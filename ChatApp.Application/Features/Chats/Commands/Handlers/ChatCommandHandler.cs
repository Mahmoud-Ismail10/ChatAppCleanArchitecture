using ChatApp.Application.Bases;
using ChatApp.Application.Features.Chats.Commands.Models;
using ChatApp.Application.Resources;
using ChatApp.Application.Services.Contracts;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Localization;

namespace ChatApp.Application.Features.Chats.Commands.Handlers
{
    public class ChatCommandHandler : ApiResponseHandler,
        IRequestHandler<CreateGroupCommand, ApiResponse<string>>,
        IRequestHandler<UpdateGroupCommand, ApiResponse<string>>,
        IRequestHandler<UpdateGroupImageCommand, ApiResponse<string>>
    {
        #region Fields
        private readonly IChatService _chatService;
        private readonly IChatMemberService _chatMemberService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        #endregion

        #region Constructors
        public ChatCommandHandler(
            IChatService chatService,
            IChatMemberService chatMemberService,
            ICurrentUserService currentUserService,
            IStringLocalizer<SharedResources> stringLocalizer) : base(stringLocalizer)
        {
            _chatService = chatService;
            _chatMemberService = chatMemberService;
            _currentUserService = currentUserService;
            _stringLocalizer = stringLocalizer;
        }
        #endregion

        #region Handle Functions
        public async Task<ApiResponse<string>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            var chat = new Chat
            {
                IsGroup = true,
                Name = request.Name,
                CreatedBy = currentUserId,
                Description = request.Description,
                CreatedAt = DateTimeOffset.UtcNow.ToLocalTime(),
                ChatMembers = new List<ChatMember>
                {
                    new ChatMember
                    {
                        UserId = currentUserId,
                        Role = Role.Owner,
                        Status = MemberStatus.Active,
                        JoinedAt = DateTimeOffset.UtcNow.ToLocalTime()
                    }
                }
            };

            foreach (var userId in request.UsersId.Distinct())
            {
                chat.ChatMembers.Add(new ChatMember
                {
                    UserId = userId,
                    Role = Role.Member,
                    Status = MemberStatus.Active,
                    JoinedAt = DateTimeOffset.UtcNow.ToLocalTime()
                });
            }

            var result = await _chatService.CreateChatAsync(chat);

            if (result != "Success")
                return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToCreateChat]);

            return Success<string>(_stringLocalizer[SharedResourcesKeys.GroupCreatedSuccessfully], chat.Id);
        }

        public async Task<ApiResponse<string>> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var IsMemberOfChat = await _chatMemberService.IsMemberOfChatAsync(currentUserId, request.ChatId);
            var IsOwnerOrAdmin = await _chatMemberService.IsOwnerOrAdminAsync(currentUserId, request.ChatId);
            if (!IsOwnerOrAdmin || !IsMemberOfChat) return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.AccessDenied]);

            var chat = await _chatService.GetChatByIdAsync(request.ChatId);
            if (chat == null) return NotFound<string>(_stringLocalizer[SharedResourcesKeys.ChatNotFound]);

            chat.Name = request.Name;
            chat.Description = request.Description;

            var result = await _chatService.UpdateChatAsync(chat);
            if (result == "Success")
                return Success<string>(_stringLocalizer[SharedResourcesKeys.GroupUpdatedSuccessfully]);
            return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToUpdateChat]);
        }

        public async Task<ApiResponse<string>> Handle(UpdateGroupImageCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var IsMemberOfChat = await _chatMemberService.IsMemberOfChatAsync(currentUserId, request.ChatId);
            var IsOwnerOrAdmin = await _chatMemberService.IsOwnerOrAdminAsync(currentUserId, request.ChatId);
            if (!IsOwnerOrAdmin || !IsMemberOfChat) return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.AccessDenied]);

            var chat = await _chatService.GetChatByIdAsync(request.ChatId);
            if (chat == null) return NotFound<string>(_stringLocalizer[SharedResourcesKeys.ChatNotFound]);

            var result = await _chatService.UpdateGroupImageAsync(chat, request.GroupImageUrl);
            return result switch
            {
                "Success" => Edit<string>(_stringLocalizer[SharedResourcesKeys.GroupImageUpdatedSuccessfully]),
                "NoImageProvided" => BadRequest<string>(_stringLocalizer[SharedResourcesKeys.NoImageProvided]),
                _ => BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToUpdateGroupImage])
            };
        }
        #endregion
    }
}
