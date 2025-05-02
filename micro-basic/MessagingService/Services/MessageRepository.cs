using System;
using System.Text.Json;
using Hazelcast.Core;
using MessagingService.Models;
using Microsoft.Extensions.Options;

namespace MessagingService.Services;

public interface IMessageRepository
{
    Task StoreMessage(Message message);

    Task<List<Message>> GetMessages();
}

public class MessageRepository(
    IHazelcastClientProvider hazelcastClientProvider,
    IConfiguration configuration,
    ILogger<MessageRepository> logger

) : IMessageRepository
{
    private readonly string _hazelcastMapName = configuration["Application:MessagesMap"] ?? "messages";

    public async Task<List<Message>> GetMessages()
    {

        var client = await hazelcastClientProvider.GetHazelcastClient();
        var map = await client.GetMapAsync<string, HazelcastJsonValue>(_hazelcastMapName);

        var messages = await map.GetValuesAsync(Hazelcast.Query.Predicates.True());
        return messages.Select(x => JsonSerializer.Deserialize<Message>(x.Value)).Where(x => x is not null).ToList();
    }

    public async Task StoreMessage(Message message)
    {
        logger.LogInformation("Saving new message {@Message}", message);
        logger.LogInformation("Getting Hazelcast client");
        var client = await hazelcastClientProvider.GetHazelcastClient();

        logger.LogInformation("Getting Hazelcast map {MapName}", _hazelcastMapName);
        var map = await client.GetMapAsync<string, HazelcastJsonValue>(_hazelcastMapName);

        var jsonValue = JsonSerializer.Serialize(message);
        var hazelcastJsonValue = new HazelcastJsonValue(jsonValue);
        await map.PutAsync(message.MessageId, hazelcastJsonValue);
        logger.LogInformation("Saved new message {@Message}", message);       
    }
}
