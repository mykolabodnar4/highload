namespace ReplicatedLog;

public class LogConfig
{
    public AppMode Mode { get; set; }
    public string[] Secondaries { get; set; } = [];
}