using ReplicatedLog.Models;

namespace ReplicatedLog.Services;

public interface IReplicationService
{
    Task ReplicateMessage(LogEntry message, int writeConcern);
}
