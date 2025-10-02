using ChatApp.Application.Bases;
using ChatApp.Application.Features.ChatsMember.Commands.Models;
using ChatApp.Application.Resources;
using ChatApp.Application.Services.Contracts;
using MediatR;
using Microsoft.Extensions.Localization;

namespace ChatApp.Application.Features.ChatsMember.Commands.Handlers
{
    public class ChatMemberCommandHandler : ApiResponseHandler,
        IRequestHandler<DeleteChatForMeCommand, ApiResponse<string>>
    {
        #region Fields
        private readonly IChatService _chatService;
        private readonly IChatMemberService _chatMemberService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        #endregion

        #region Constructors
        public ChatMemberCommandHandler(
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
        public async Task<ApiResponse<string>> Handle(DeleteChatForMeCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var chatMember = await _chatMemberService.GetChatMemberByIdAsync(request.ChatMemberId);

            if (chatMember == null) return NotFound<string>(_stringLocalizer[SharedResourcesKeys.ChatNotFound]);
            var result = await _chatMemberService.SoftDeleteChatMemberAsync(chatMember);
            if (result != "Success") return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToDeleteChat]);

            var isDeletedFromAll = await _chatMemberService.IsDeletedFromAllMembersAsync(chatMember.ChatId);
            if (isDeletedFromAll)
            {
                if (chatMember.Chat == null) return NotFound<string>(_stringLocalizer[SharedResourcesKeys.ChatNotFound]);
                result = await _chatService.DeleteChatAsync(chatMember.Chat);
                if (result != "Success") return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToDeleteChat]);
            }
            return Success<string>(_stringLocalizer[SharedResourcesKeys.ChatDeletedForMeSuccessfully]);
        }
        #endregion
    }
}
