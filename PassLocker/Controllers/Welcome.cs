using Microsoft.AspNetCore.Mvc;

namespace PassLocker.Controllers
{
    [Route("api/[Controller]")]
    public class WelcomeController : ControllerBase
    {
        [Route("welcome")]
        public IActionResult WelcomeMessage()
        {
            return Ok("Hello and welcome to the PassLocker API. Things seems to be working fine.");
        }
    }
}