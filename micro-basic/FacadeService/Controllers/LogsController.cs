using FacadeApi.Grpc.Logging;
using FacadeService.Clients;
using FacadeService.Services;
using Microsoft.AspNetCore.Mvc;

using MessageLoggerClient = FacadeApi.Grpc.Logging.MessageLogger.MessageLoggerClient;

namespace FacadeService.Controllers;

[ApiController]
[Route("api/logs")]
public class LogsController(
    MessagingApiClient messagingServiceClient,
    MessageLoggerClient loggerServiceClient,
    IKafkaPublisher kafkaPublisher,
    ILogger<LogsController> logger) 
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post(string message)
    {
        var messageObj = new Message
        {
            MessageId = Guid.NewGuid().ToString(),
            MessageText = message,
        };

        var grpcCall = loggerServiceClient.LogMessageAsync(new LogMessageRequest { Message = messageObj });
        var r = await grpcCall.ResponseAsync;
        var h = await grpcCall.ResponseHeadersAsync;
        logger.LogInformation("Response from Logging service: {@Response}", r);
        logger.LogInformation("Response headers from Logging service: {@Headers}", h);
        await kafkaPublisher.Publish(messageObj);

        return Ok(messageObj);
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var getLoggedMessagesRequest = new GetMessagesRequest();
        var getLoggerMessagesResponse = await loggerServiceClient.GetMessagesAsync(getLoggedMessagesRequest).ResponseAsync;
        var messages = getLoggerMessagesResponse.Messages;

        var getPublishedMessagesResponse = await messagingServiceClient.GetMessages();
        return Ok(messages + " - " + getPublishedMessagesResponse);
    }
}