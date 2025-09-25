using ChatApp.Presentation.Base;
using Microsoft.AspNetCore.Authorization;

namespace ChatApp.Presentation.Controllers
{
    [Authorize(AuthenticationSchemes = "SessionKey")]
    public class ChatController : AppControllerBase
    {

    }
}
