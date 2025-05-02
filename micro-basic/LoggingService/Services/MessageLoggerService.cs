using Grpc.Core;

namespace LoggingApi.Grpc;

public class MessageLoggerService(
    ILogRepository repository,
    ILogger<MessageLoggerService> logger) 
    : MessageLogger.MessageLoggerBase 
{
    public override async Task<LogMessageResponse> LogMessage(LogMessageRequest request, ServerCallContext context)
    {
        var message = request.Message; 
        logger.LogInformation("Saving new message {@Message}", message);
        var value = await repository.SaveMessage(message);
        logger.LogInformation("Saved new message {@Message}", value);

        var response = new LogMessageResponse { MessageId = message.MessageId };
        return response;
    }

    public override async Task<GetMessagesResponse> GetMessages(GetMessagesRequest request, ServerCallContext context)
    {
        logger.LogInformation("Getting messages");
        var messages = await repository.GetAllMessages();
        var response = new GetMessagesResponse
        {
            Messages = string.Join("; ", messages.Select(message => message.MessageText)),
        };
        logger.LogInformation("Returning {Count} messages", messages.Count);
        return response;
    }
}
