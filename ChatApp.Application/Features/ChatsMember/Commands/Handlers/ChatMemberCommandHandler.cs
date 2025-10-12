using ChatApp.Application.Bases;
using ChatApp.Application.Features.ChatsMember.Commands.Models;
using ChatApp.Application.Resources;
using ChatApp.Application.Services.Contracts;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Localization;

namespace ChatApp.Application.Features.ChatsMember.Commands.Handlers
{
    public class ChatMemberCommandHandler : ApiResponseHandler,
        IRequestHandler<DeleteChatForMeCommand, ApiResponse<string>>,
        IRequestHandler<PinOrUnpinChatCommand, ApiResponse<string>>,
        IRequestHandler<MakeAsAdminOrUnadminCommand, ApiResponse<string>>,
        IRequestHandler<RemoveMemberFromGroupCommand, ApiResponse<string>>,
        IRequestHandler<AddMembersToGroupCommand, ApiResponse<string>>
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

        public async Task<ApiResponse<string>> Handle(MakeAsAdminOrUnadminCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var chatMember = await _chatMemberService.GetChatMemberByIdAsync(request.ChatMemberId);
            if (chatMember == null) return NotFound<string>(_stringLocalizer[SharedResourcesKeys.MemberNotFound]);

            var IsMemberOfChat = await _chatMemberService.IsMemberOfChatAsync(currentUserId, chatMember.ChatId);
            var IsOwnerOrAdmin = await _chatMemberService.IsOwnerOrAdminAsync(currentUserId, chatMember.ChatId);
            if (!IsMemberOfChat || !IsMemberOfChat) return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.AccessDenied]);

            if (chatMember.Role == Role.Owner)
                return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.CannotChangeOwnerRole]);


            var response = await _chatMemberService.MakeAsAdminOrUnadminAsync(chatMember);
            if (response == "Success")
                return Success<string>(_stringLocalizer[SharedResourcesKeys.MemberRoleChangedSuccessfully]);
            return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToChangeMemberRole]);
        }

        public async Task<ApiResponse<string>> Handle(RemoveMemberFromGroupCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var chatMember = await _chatMemberService.GetChatMemberByIdAsync(request.ChatMemberId);
            if (chatMember == null) return NotFound<string>(_stringLocalizer[SharedResourcesKeys.MemberNotFound]);

            var IsMemberOfChat = await _chatMemberService.IsMemberOfChatAsync(currentUserId, chatMember.ChatId);
            var IsOwnerOrAdmin = await _chatMemberService.IsOwnerOrAdminAsync(currentUserId, chatMember.ChatId);
            if (!IsOwnerOrAdmin || !IsMemberOfChat) return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.AccessDenied]);

            if (chatMember.Role == Role.Owner)
                return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.CannotRemoveOwnerFromGroup]);

            var response = await _chatMemberService.RemoveMemberFromGroupAsync(chatMember);
            if (response == "Success")
                return Success<string>(_stringLocalizer[SharedResourcesKeys.MemberRemovedFromGroupSuccessfully]);
            return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToRemoveMemberFromGroup]);
        }

        public async Task<ApiResponse<string>> Handle(AddMembersToGroupCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var chat = await _chatService.GetChatByIdAsync(request.ChatId);
            if (chat == null) return NotFound<string>(_stringLocalizer[SharedResourcesKeys.MemberNotFound]);

            var IsMemberOfChat = await _chatMemberService.IsMemberOfChatAsync(currentUserId, request.ChatId);
            var IsOwnerOrAdmin = await _chatMemberService.IsOwnerOrAdminAsync(currentUserId, request.ChatId);
            if (!IsOwnerOrAdmin || !IsMemberOfChat) return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.AccessDenied]);

            var existingMemberIds = chat.ChatMembers.Where(cm => cm.Status == MemberStatus.Active)
                                                    .Select(cm => cm.UserId)
                                                    .ToHashSet();
            var newMembersIds = request.UserIds.Where(id => !existingMemberIds.Contains(id)).ToList();
            if (!newMembersIds.Any())
                return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.NoNewMembersToAdd]);

            foreach (var userId in newMembersIds)
            {
                var existingMember = chat.ChatMembers.FirstOrDefault(cm => cm.UserId == userId);

                if (existingMember != null)
                {
                    if (existingMember.Status != MemberStatus.Active)
                    {
                        existingMember.Status = MemberStatus.Active;
                        existingMember.JoinedAt = DateTimeOffset.UtcNow.ToLocalTime();
                    }
                    continue;
                }
                chat.ChatMembers.Add(new ChatMember
                {
                    UserId = userId,
                    Role = Role.Member,
                    Status = MemberStatus.Active,
                    JoinedAt = DateTimeOffset.UtcNow.ToLocalTime()
                });
            }
            var response = await _chatService.UpdateChatAsync(chat);
            if (response == "Success")
                return Success<string>(_stringLocalizer[SharedResourcesKeys.MembersAddedToGroupSuccessfully]);
            return BadRequest<string>(_stringLocalizer[SharedResourcesKeys.FailedToAddMembersToGroup]);
        }
        #endregion
    }
}
