using ReplicatedLog.Models;

namespace ReplicatedLog.Services;

public class InMemoryLogRepository(
    SortedDictionary<int, Message> dictionary) : ILogRepository
{
    public Message SaveMessage(LogEntry entry)
    {
        lock (dictionary)
        {
            dictionary[entry.GlobalSequenceNumber] = entry.Message;
        }

        return entry.Message;
    }

    public List<Message> GetMessages()
    {
        List<Message> ordered = new();
        int nextExpected = 1;
        lock (dictionary)
        {
            while (dictionary.TryGetValue(nextExpected, out var msg))
            {
                ordered.Add(msg);
                nextExpected++;
            }
        }

        return ordered;
    }
}