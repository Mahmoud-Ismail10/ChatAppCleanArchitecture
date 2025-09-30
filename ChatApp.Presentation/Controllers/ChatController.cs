using ChatApp.Application.Features.Chats.Queries.Models;
using ChatApp.Domain.AppMetaData;
using ChatApp.Presentation.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.Controllers
{
    [Authorize(AuthenticationSchemes = "SessionKey")]
    public class ChatController : AppControllerBase
    {
        [HttpGet(Router.Chat.GetChatWithMessages)]
        public async Task<IActionResult> GetChatWithMessages([FromQuery] GetChatWithMessagesQuery query)
        {
            var result = await Mediator.Send(query);
            return NewResult(result);
        }
    }
}
