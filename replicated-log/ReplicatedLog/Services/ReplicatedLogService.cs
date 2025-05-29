using ReplicatedLog.Models;

namespace ReplicatedLog.Services;

public class ReplicatedLogService(
    ILogRepository logRepository,
    IReplicationService replicationService) : ILogService
{
    public async Task Store(LogEntry message, int writeConcern = 1)
    {
        logRepository.SaveMessage(message);
        await replicationService.ReplicateMessage(message, writeConcern);
    }

    public List<Message> GetMessages()
    {
        return logRepository.GetMessages();
    }
}
