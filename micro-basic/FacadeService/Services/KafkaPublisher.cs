using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace FacadeService.Services;

public class KafkaPublisher(
    IProducer<string, string> producer, 
    IConfiguration configuration) : IKafkaPublisher
{
    private readonly string _topic = configuration["Kafka:Topic"] 
            ?? throw new ArgumentException("Kafka:Topic configuration is missing or empty.");


    public async Task Publish(FacadeApi.Grpc.Logging.Message message)
    {
        var outgoingMessage = new Message<string, string>
        {
            Key = message.MessageId,
            Value = JsonSerializer.Serialize(message),
        };

        try
        {
            var deliveryResult = await producer.ProduceAsync(_topic, outgoingMessage);

            Console.WriteLine($"Delivered '{deliveryResult.Value}' to '{deliveryResult.TopicPartitionOffset}'");
        }
        catch (ProduceException<Null, string> ex)
        {
            Console.WriteLine($"Error delivering message: {ex.Error.Reason}");
        }
    }
}