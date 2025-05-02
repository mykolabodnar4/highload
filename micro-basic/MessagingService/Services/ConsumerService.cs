using System.Text.Json;
using Confluent.Kafka;

namespace MessagingService.Services;

public class ConsumerService(
    IConsumer<string, string> consumer,
    IMessageRepository messageRepository,
    IConfiguration configuration,
    ILogger<ConsumerService> logger
) : BackgroundService
{
    private readonly string _topic = configuration["Kafka:Topic"] ?? throw new ArgumentException("Kafka:Topic configuration is missing or empty.");

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() => StartConsumerLoop(stoppingToken), stoppingToken);
    }

    private async Task StartConsumerLoop(CancellationToken stoppingToken)
    {
        consumer.Subscribe(_topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);
                    if (result.IsPartitionEOF) continue;

                    try 
                    {
                        // Process the message
                        var message = result.Message.Value;
                        logger.LogInformation($"Consumed message '{message}' from topic '{_topic}'.");
                        var deserializedMessage = JsonSerializer.Deserialize<Models.Message>(message);
                        if (deserializedMessage == null)
                        {
                            logger.LogWarning("Deserialized message is null.");
                            continue;
                        }
                        await messageRepository.StoreMessage(deserializedMessage);

                        // Commit the offset after processing the message
                        consumer.Commit(result);
                    } 
                    catch (Exception e) 
                    {
                        logger.LogError(e, "Error while processing message.");
                    }
                }
                catch (ConsumeException e)
                {
                    logger.LogError(e, "Error occurred while consuming messages.");
                    if (e.Error.IsFatal)
                    {
                        // https://github.com/edenhill/librdkafka/blob/master/INTRODUCTION.md#fatal-consumer-errors
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unexpected error: {e}");
                    break;
                }
            }
        }
        finally
        {
            // Ensure the consumer leaves the group cleanly and final offsets are committed.
            consumer.Close();
        }
    }
}
