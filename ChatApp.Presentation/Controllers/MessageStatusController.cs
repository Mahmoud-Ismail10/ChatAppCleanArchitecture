using ChatApp.Application.Features.MessageStatuses.Queries.Models;
using ChatApp.Domain.AppMetaData;
using ChatApp.Presentation.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.Controllers
{
    [Authorize(AuthenticationSchemes = "SessionKey")]
    public class MessageStatusController : AppControllerBase
    {
        [HttpGet(Router.MessageStatus.GetMessageStatuses)]
        public async Task<IActionResult> GetMessageStatuses([FromRoute] Guid id)
        {
            var result = await Mediator.Send(new GetMessageStatusesQuery(id));
            return NewResult(result);
        }
    }
}
