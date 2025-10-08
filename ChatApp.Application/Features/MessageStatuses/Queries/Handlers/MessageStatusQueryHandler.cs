using ChatApp.Application.Bases;
using ChatApp.Application.Features.MessageStatuses.Queries.Models;
using ChatApp.Application.Features.MessageStatuses.Queries.Responses;
using ChatApp.Application.Resources;
using ChatApp.Application.Services.Contracts;
using MediatR;
using Microsoft.Extensions.Localization;

namespace ChatApp.Application.Features.MessageStatuses.Queries.Handlers
{
    public class MessageStatusQueryHandler : ApiResponseHandler,
        IRequestHandler<GetMessageStatusesQuery, ApiResponse<List<MessageStatusDto>>>
    {
        #region Fields
        private readonly IMessageService _messageService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMessageStatusService _messageStatusService;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        #endregion

        #region Constructors
        public MessageStatusQueryHandler(
            IMessageService messageService,
            ICurrentUserService currentUserService,
            IMessageStatusService messageStatusService,
            IStringLocalizer<SharedResources> stringLocalizer) : base(stringLocalizer)
        {
            _messageService = messageService;
            _currentUserService = currentUserService;
            _messageStatusService = messageStatusService;
            _stringLocalizer = stringLocalizer;
        }
        #endregion

        #region Handle Functions
        public async Task<ApiResponse<List<MessageStatusDto>>> Handle(GetMessageStatusesQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var message = await _messageService.GetMessageByIdAsync(request.MessageId);
            if (message == null)
                return NotFound<List<MessageStatusDto>>(_stringLocalizer[SharedResourcesKeys.MessageNotFound]);
            if (message.SenderId != currentUserId)
                return Unauthorized<List<MessageStatusDto>>(_stringLocalizer[SharedResourcesKeys.UnauthorizedToGetMessageStatuses]);
            var statuses = await _messageStatusService.GetMessageStatusesAsync(request.MessageId);
            return Success(statuses);
        }
        #endregion
    }
}
