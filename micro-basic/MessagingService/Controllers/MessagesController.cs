using MessagingService.Services;
using Microsoft.AspNetCore.Mvc;

namespace MessagingService.Controllers;

[ApiController]
[Route("api/messages")]
public class MessagesController(
    IMessageRepository messageRepository
) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var messages = await messageRepository.GetMessages();
        return Ok(messages);
    }
}