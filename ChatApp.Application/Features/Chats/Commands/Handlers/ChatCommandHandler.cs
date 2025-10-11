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
        IRequestHandler<CreateGroupCommand, ApiResponse<string>>
    {
        #region Fields
        private readonly IChatService _chatService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        #endregion

        #region Constructors
        public ChatCommandHandler(
            IChatService chatService,
            ICurrentUserService currentUserService,
            IStringLocalizer<SharedResources> stringLocalizer) : base(stringLocalizer)
        {
            _chatService = chatService;
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
                        Role = Role.Admin,
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
        #endregion
    }
}
