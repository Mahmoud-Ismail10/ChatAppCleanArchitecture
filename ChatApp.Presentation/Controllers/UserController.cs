using ChatApp.Application.Features.Users.Queries.Models;
using ChatApp.Domain.AppMetaData;
using ChatApp.Presentation.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.Controllers
{
    [Authorize(AuthenticationSchemes = "SessionKey")]
    public class UserController : AppControllerBase
    {
        [HttpGet(Router.User.GetUserStatus)]
        public async Task<IActionResult> GetUserStatus([FromRoute] Guid id)
        {
            var result = await Mediator.Send(new GetUserStatusQuery(id));
            return NewResult(result);
        }
    }
}
