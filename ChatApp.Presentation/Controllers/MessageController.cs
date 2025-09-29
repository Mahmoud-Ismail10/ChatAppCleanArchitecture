using ChatApp.Application.Features.Messages.Commands.Models;
using ChatApp.Domain.AppMetaData;
using ChatApp.Presentation.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.Controllers
{
    public class MessageController : AppControllerBase
    {
        [Authorize(AuthenticationSchemes = "SessionKey")]
        [HttpPost(Router.Message.SendMessageToContact)]
        public async Task<IActionResult> SendMessageToContact([FromForm] SendMessageToContactCommand command)
        {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }
    }
}
