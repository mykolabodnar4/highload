using System.Collections.Concurrent;

namespace LoggingApi.Grpc;

public interface ILogRepository
{
    Task<Message> SaveMessage(Message message);
    Task<List<Message>> GetMessages();
}

public class InMemoryLogRepository(ConcurrentDictionary<string, Message> dictionary) : ILogRepository
{
    public Task<Message> SaveMessage(Message message)
    {
        var value = dictionary.GetOrAdd(message.MessageId, _ => message);
        return Task.FromResult(value);
    }

    public Task<List<Message>> GetMessages()
    {
        var messages = dictionary.Values.ToList();
        return Task.FromResult(messages);
    }
}