using ChatApp.Application.Features.ChatsMember.Queries.Models;
using ChatApp.Domain.AppMetaData;
using ChatApp.Presentation.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.Controllers
{
    public class ChatMemberController : AppControllerBase
    {
        [Authorize(AuthenticationSchemes = "SessionKey")]
        [HttpGet(Router.ChatMember.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var result = await Mediator.Send(new GetAllChatsMemberQuery());
            return NewResult(result);
        }
    }
}
