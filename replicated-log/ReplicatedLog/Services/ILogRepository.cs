using ReplicatedLog.Models;

namespace ReplicatedLog.Services;

public interface ILogRepository
{
    Message SaveMessage(LogEntry message);
    List<Message> GetMessages();
}