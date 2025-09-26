using ChatApp.Application.Bases;
using ChatApp.Application.Features.ChatsMember.Queries.Models;
using ChatApp.Application.Features.ChatsMember.Queries.Responses;
using ChatApp.Application.Resources;
using ChatApp.Application.Services.Contracts;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System.Security.Claims;

namespace ChatApp.Application.Features.ChatsMember.Queries.Handlers
{
    public class ChatMemberCommandHandler : ApiResponseHandler,
        IRequestHandler<GetAllChatsMemberQuery, ApiResponse<List<GetAllChatsMemberResponse>>>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IChatMemberService _chatMemberService;
        #endregion

        #region Constructors
        public ChatMemberCommandHandler(IStringLocalizer<SharedResources> stringLocalizer,
            IHttpContextAccessor httpContextAccessor,
            IChatMemberService chatMemberService) : base(stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
            _httpContextAccessor = httpContextAccessor;
            _chatMemberService = chatMemberService;
        }
        #endregion

        #region Handle Functions
        public async Task<ApiResponse<List<GetAllChatsMemberResponse>>> Handle(GetAllChatsMemberQuery request, CancellationToken cancellationToken)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
                return Unauthorized<List<GetAllChatsMemberResponse>>(_stringLocalizer[SharedResourcesKeys.UnAuthorized]);

            var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var chatMembers = await _chatMemberService.GetAllChatsMemberAsync(Guid.Parse(userId!));
            if (chatMembers == null || !chatMembers.Any())
                return NotFound<List<GetAllChatsMemberResponse>>(_stringLocalizer[SharedResourcesKeys.NoChatsFound]);
            var responses = chatMembers.Select(m => new GetAllChatsMemberResponse(
                                           m!.Chat!.Id,
                                           m.Chat.IsGroup,
                                           m.Chat.Name,
                                           m.Chat.GroupImageUrl,
                                           m.Chat.LastMessage != null ? m.Chat.LastMessage.Type : null,
                                           m.Chat.LastMessage != null ? m.Chat.LastMessage.Content : null,
                                           m.Chat.LastMessage != null ? m.Chat.LastMessage.SentAt : null
                                       ))
                                       .ToList();

            return Success(responses);
        }
        #endregion
    }
}
