using ChatApp.Application.Features.Authentication.Commands.Models;
using ChatApp.Domain.AppMetaData;
using ChatApp.Presentation.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.Controllers
{
    public class AuthenticationController : AppControllerBase
    {
        [HttpPost(Router.Authentication.SendOtp)]
        public async Task<IActionResult> SendOtp([FromQuery] SendOtpCommand command)
        {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }

        [HttpPost(Router.Authentication.VerifyOtp)]
        public async Task<IActionResult> VerifyOtp([FromQuery] VerifyOtpCommand command)
        {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }

        [HttpPost(Router.Authentication.Register)]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }

        [HttpPost(Router.Authentication.CreateSession)]
        public async Task<IActionResult> CreateSession([FromQuery] CreateSessionCommand command)
        {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }

        [Authorize(AuthenticationSchemes = "SessionKey")]
        [HttpPost(Router.Authentication.Logout)]
        public async Task<IActionResult> Logout()
        {
            var result = await Mediator.Send(new LogoutCommand());
            return NewResult(result);
        }
    }
}
