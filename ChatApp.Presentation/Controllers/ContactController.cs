using ChatApp.Application.Features.Contacts.Commands.Models;
using ChatApp.Application.Features.Contacts.Queries.Models;
using ChatApp.Domain.AppMetaData;
using ChatApp.Presentation.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.Controllers
{
    [Authorize(AuthenticationSchemes = "SessionKey")]
    public class ContactController : AppControllerBase
    {
        [HttpPost(Router.Contact.AddToContactsByPhoneNumber)]
        public async Task<IActionResult> AddToContactsByPhoneNumber([FromQuery] AddToContactsByPhoneNumberCommand command)
        {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }

        [HttpGet(Router.Contact.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var result = await Mediator.Send(new GetAllContactsQuery());
            return NewResult(result);
        }

        [HttpGet(Router.Contact.ViewContact)]
        public async Task<IActionResult> ViewContact([FromQuery] ViewContactQuery query)
        {
            var result = await Mediator.Send(query);
            return NewResult(result);
        }
    }
}
