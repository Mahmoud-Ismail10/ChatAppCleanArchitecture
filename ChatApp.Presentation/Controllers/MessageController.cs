using ChatApp.Application.Features.Messages.Commands.Models;
using ChatApp.Domain.AppMetaData;
using ChatApp.Presentation.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.Controllers
{
    [Authorize(AuthenticationSchemes = "SessionKey")]
    public class MessageController : AppControllerBase
    {
        [HttpPost(Router.Message.SendMessageToContact)]
        public async Task<IActionResult> SendMessageToContact([FromForm] SendMessageToContactCommand command)
        {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }

        [HttpDelete(Router.Message.DeleteMessage)]
        public async Task<IActionResult> DeleteMessage([FromRoute] Guid id)
        {
            var result = await Mediator.Send(new DeleteMessageCommand(id));
            return NewResult(result);
        }
    }
}
