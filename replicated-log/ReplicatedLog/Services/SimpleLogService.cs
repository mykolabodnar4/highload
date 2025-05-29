using ReplicatedLog.Models;

namespace ReplicatedLog.Services;

public class SimpleLogService(
    ILogRepository logRepository) : ILogService
{
    public Task Store(LogEntry logEntry, int _)
    {
        logRepository.SaveMessage(logEntry);
        return Task.CompletedTask;
    }

    public List<Message> GetMessages()
    {
        return logRepository.GetMessages();
    }
}