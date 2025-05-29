using ReplicatedLog.Models;

namespace ReplicatedLog.Services;

public interface ILogService
{
    Task Store(LogEntry message, int writeConcern = 1);
    List<Message> GetMessages();
}
