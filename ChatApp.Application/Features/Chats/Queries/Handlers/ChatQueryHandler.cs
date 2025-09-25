using ChatApp.Application.Bases;
using ChatApp.Application.Features.Chats.Queries.Models;
using ChatApp.Application.Features.Chats.Queries.Responses;
using ChatApp.Application.Resources;
using ChatApp.Application.Services.Contracts;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace ChatApp.Application.Features.Chats.Queries.Handlers
{
    public class ChatQueryHandler : ApiResponseHandler,
        IRequestHandler<GetChatWithMessagesQuery, ApiResponse<GetChatWithMessagesResponse>>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        private readonly IChatService _chatService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Constructors
        public ChatQueryHandler(IStringLocalizer<SharedResources> stringLocalizer,
            IChatService chatService,
            IHttpContextAccessor httpContextAccessor) : base(stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
            _chatService = chatService;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region Handle Functions
        public async Task<ApiResponse<GetChatWithMessagesResponse>> Handle(GetChatWithMessagesQuery request, CancellationToken cancellationToken)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
                return Unauthorized<GetChatWithMessagesResponse>(_stringLocalizer[SharedResourcesKeys.UnAuthorized]);

            var chat = await _chatService.GetChatWithMessagesAsync(request.ChatId);
            if (chat == null)
                return NotFound<GetChatWithMessagesResponse>(_stringLocalizer[SharedResourcesKeys.ChatNotFound]);

            var response = new GetChatWithMessagesResponse(
                chat.Id,
                chat.IsGroup,
                chat.Name,
                chat.GroupImageUrl,
                chat.Messages.Select(m => new MessageDto(
                    m.Id,
                    m.SenderId,
                    m.Type,
                    m.Content,
                    m.SentAt
                )).ToList()
            );
            return Success(response);
        }
        #endregion
    }
}
