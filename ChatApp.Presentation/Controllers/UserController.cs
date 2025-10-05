using ChatApp.Application.Features.Users.Commands.Models;
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

        [HttpGet(Router.User.GetCurrentUser)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var result = await Mediator.Send(new GetCurrentUserQuery());
            return NewResult(result);
        }

        [HttpGet(Router.User.GetCurrentUserId)]
        public async Task<IActionResult> GetCurrentUserId()
        {
            var result = await Mediator.Send(new GetCurrentUserIdQuery());
            return NewResult(result);
        }

        [HttpPut(Router.User.UpdateProfile)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command)
        {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }

        [HttpPut(Router.User.UpdateProfileImage)]
        public async Task<IActionResult> UpdateProfileImage([FromForm] UpdateProfileImageCommand command)
        {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }

        [HttpPut(Router.User.DeleteProfileImage)]
        public async Task<IActionResult> UpdateProfileImage()
        {
            var result = await Mediator.Send(new DeleteProfileImageCommand());
            return NewResult(result);
        }
    }
}
