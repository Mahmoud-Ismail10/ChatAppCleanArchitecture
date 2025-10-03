using ChatApp.Application.Bases;
using ChatApp.Application.Features.ChatsMember.Commands.Models;
using ChatApp.Application.Resources;
using ChatApp.Application.Services.Contracts;
using MediatR;
using Microsoft.Extensions.Localization;

namespace ChatApp.Application.Features.ChatsMember.Commands.Handlers
{
    public class ChatMemberCommandHandler : ApiResponseHandler,
        IRequestHandler<DeleteChatForMeCommand, ApiResponse<string>>,
        IRequestHandler<PinOrUnpinChatCommand, ApiResponse<string>>
    {
        #region Fields
        private readonly IChatService _chatService;
        private readonly IChatMemberService _chatMemberService;
        private readonly ITransactionService _transactionService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        #endregion

        #region Constructors
        public ChatMemberCommandHandler(
            IChatService chatService,
            IChatMemberService chatMemberService,
            ITransactionService transactionService,
            ICurrentUserService currentUserService,
            IStringLocalizer<SharedResources> stringLocalizer) : base(stringLocalizer)
        {
            _chatService = chatService;
            _chatMemberService = chatMemberService;
            _transactionService = transactionService;
            _currentUserService = currentUserService;
            _stringLocalizer = stringLocalizer;
        }
        #endregion

        #region Handle Functions
        public async Task<ApiResponse<string>> Handle(DeleteChatForMeCommand request, CancellationToken cancellationToken)
        {
            using var transaction = await _transactionService.BeginTransactionAsync();

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
                if (result != "Success")
                {
                    await _transactionService.RollBackAsync();
                    return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToDeleteChat]);
                }
            }
            await _transactionService.CommitAsync();
            return Deleted<string>(_stringLocalizer[SharedResourcesKeys.ChatDeletedForMeSuccessfully]);
        }

        public async Task<ApiResponse<string>> Handle(PinOrUnpinChatCommand request, CancellationToken cancellationToken)
        {
            _currentUserService.IsAuthenticated();
            var response = await _chatMemberService.PinOrUnpinChatAsync(request.ChatMemberId);
            return response switch
            {
                "Failed" => BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToPinOrUnpinChat]),
                "ChatMemberNotFound" => NotFound<string>(_stringLocalizer[SharedResourcesKeys.ChatNotFound]),
                _ => Success<string>(_stringLocalizer[SharedResourcesKeys.ChatPinStatusChangedSuccessfully])
            };
        }
        #endregion
    }
}
