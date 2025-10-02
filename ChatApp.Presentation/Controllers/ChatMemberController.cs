using ChatApp.Application.Features.ChatsMember.Commands.Models;
using ChatApp.Application.Features.ChatsMember.Queries.Models;
using ChatApp.Domain.AppMetaData;
using ChatApp.Presentation.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.Controllers
{
    [Authorize(AuthenticationSchemes = "SessionKey")]
    public class ChatMemberController : AppControllerBase
    {
        [HttpGet(Router.ChatMember.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var result = await Mediator.Send(new GetAllChatsMemberQuery());
            return NewResult(result);
        }

        [HttpDelete(Router.ChatMember.DeleteForMe)]
        public async Task<IActionResult> DeleteForMe([FromRoute] Guid id)
        {
            var result = await Mediator.Send(new DeleteChatForMeCommand(id));
            return NewResult(result);
        }
    }
}

