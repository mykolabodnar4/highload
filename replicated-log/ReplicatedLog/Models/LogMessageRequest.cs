namespace ReplicatedLog.Models;

public record LogMessageRequest
{
    public required string Message { get; set; }
    public required int WriteConcern { get; set; } = 1;
}
