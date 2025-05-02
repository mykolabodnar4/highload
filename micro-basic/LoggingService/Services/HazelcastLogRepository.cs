using System.Text.Json;
using Hazelcast.Core;
using LoggingService;
using LoggingService.Services;
using Microsoft.Extensions.Options;

namespace LoggingApi.Grpc;

public class HazelcastLogRepository(
    IHazelcastClientProvider hazelcastClientProvider,
    IOptions<ApplicationSettings> options,
    ILogger<HazelcastLogRepository> logger
) : ILogRepository
{

    public async Task<Message> SaveMessage(Message message)
    {
        logger.LogInformation("Saving new message {@Message}", message);
        logger.LogInformation("Getting Hazelcast client");        
        var client = await hazelcastClientProvider.GetHazelcastClient();

        logger.LogInformation("Getting Hazelcast map {MapName}", options.Value.MessagesMap);
        var map = await client.GetMapAsync<string, HazelcastJsonValue>(options.Value.MessagesMap);
        
        var jsonValue = JsonSerializer.Serialize(message);
        var hazelcastJsonValue = new HazelcastJsonValue(jsonValue);
        await map.PutAsync(message.MessageId, hazelcastJsonValue);
        logger.LogInformation("Saved new message {@Message}", message);
        return message;
    }

    public async Task<List<Message>> GetAllMessages()
    {
        var client = await hazelcastClientProvider.GetHazelcastClient();
        var map = await client.GetMapAsync<string, HazelcastJsonValue>(options.Value.MessagesMap);
        
        var messages = await map.GetValuesAsync(Hazelcast.Query.Predicates.True());
        return messages.Select(x => JsonSerializer.Deserialize<Message>(x.Value)).Where(x => x is not null).ToList();
    }
} 
