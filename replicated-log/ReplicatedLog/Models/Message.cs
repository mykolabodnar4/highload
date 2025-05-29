namespace ReplicatedLog;

public record Message
{
    public required string MessageId { get; set; }
    public required string Text { get; set; }
}