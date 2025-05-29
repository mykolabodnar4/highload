using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ReplicatedLog.Models;

namespace ReplicatedLog.Services;

public class ReplicationService(
    HttpClient httpClient,
    IOptions<LogConfig> logConfigOptions,
    ILogger<ReplicationService> logger) : IReplicationService
{
    public async Task ReplicateMessage(LogEntry message, int writeConcern)
    {
        const string path = "/replication/messages";
        var logConfig = logConfigOptions.Value;
        if(logConfig.Secondaries.Length == 0)
        {
            logger.LogWarning("No secondary nodes configured for replication.");
            return;
        }

        writeConcern = Math.Min(writeConcern, logConfig.Secondaries.Length);

        var tasks = logConfig.Secondaries
            .Select(secondary => Post(secondary + path, message))
            .ToList();

        var completed = new List<Task<(bool, string)>>();
        while (completed.Count < writeConcern - 1 && tasks.Count > 0)
        {
            var task = await Task.WhenAny(tasks);
            tasks.Remove(task);

            try
            {
                var result = await task;
                completed.Add(task);
                logger.LogInformation($"Replication to {result.url} succeeded: {result.success}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Replication error: {ex.Message}");
            }
        }

        logger.LogInformation("Sending remaining requests in background...");
        _ = Task.WhenAll(tasks).ContinueWith(_ =>
        {
            logger.LogInformation("Remaining requests finished in background.");
        });
    }

    private async Task<(bool success, string url)> Post(string url, LogEntry message)
    {
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Post,
            Content = new StringContent(
                JsonSerializer.Serialize(message),
                Encoding.UTF8,
                "application/json")
        };
        var response = await httpClient.SendAsync(request);
        return (response.IsSuccessStatusCode, url);
    }
}