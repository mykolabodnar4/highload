using System;

namespace ReplicatedLog.Models;

public record LogEntry
{
    public required int GlobalSequenceNumber { get; set; }
    public required Message Message { get; set; }
}
