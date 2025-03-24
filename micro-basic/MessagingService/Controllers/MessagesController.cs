using Microsoft.AspNetCore.Mvc;

namespace MessagingService.Controllers;

[ApiController]
[Route("api/messages")]
public class MessagesController : ControllerBase
{
    [HttpPost]
    public IActionResult Post(object message)
    {
        return Ok("Not implemented");
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Not implemented");
    }
}