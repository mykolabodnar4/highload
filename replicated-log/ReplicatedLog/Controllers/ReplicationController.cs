using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ReplicatedLog.Models;
using ReplicatedLog.Services;

namespace ReplicatedLog;

[Route("replication")]
[ApiController]
public class ReplicationController(
    ILogService logService,
    ILogger<ReplicationController> logger) : ControllerBase
{
    [HttpPost("messages")]
    public async Task<IActionResult> Post(LogEntry logEntry)
    {
        logger.LogInformation("Received log entry with GlobalSequenceNumber: {GlobalSequenceNumber}", logEntry.GlobalSequenceNumber);
        // introduce a random delay to simulate network latency
        Random random = new Random();
        int delay = random.Next(50, 1500);
        logger.LogInformation("Delaying for {Delay} milliseconds", delay);
        await Task.Delay(delay);

        await logService.Store(logEntry);

        return Ok();
    }
}
