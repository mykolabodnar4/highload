using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ReplicatedLog.Models;
using ReplicatedLog.Services;

namespace ReplicatedLog;

[Route("/")]
[ApiController]
public class LogsController(
    ILogService logService,
    GlobalSequencer globalSequencer,
    IOptions<LogConfig> logConfigOptions) : ControllerBase
{
    [HttpPost("messages")]
    public async Task<IActionResult> Post(LogMessageRequest request)
    {
        if (logConfigOptions.Value.Mode == AppMode.Secondary)
        {
            return BadRequest();
        }
        var logEntry = new LogEntry
        {
            GlobalSequenceNumber = globalSequencer.NextSequenceNumber,
            Message = new Message
            {
                MessageId = Guid.NewGuid().ToString(),
                Text = request.Message
            }
        };

        await logService.Store(logEntry, request.WriteConcern);
        return Ok();
    }

    [HttpGet("messages")]
    public IActionResult GetMessages()
    {
        return Ok(logService.GetMessages());
    }
}