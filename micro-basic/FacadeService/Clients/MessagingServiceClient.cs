using System.Text;
using System.Text.Json;
using FacadeApi.Grpc.Logging;

namespace FacadeService.Clients;

public class MessagingServiceClient(HttpClient httpClient, ILogger<MessagingServiceClient> logger)
{
    public async Task PublishMessage(Message message)
    {
        string path = "messages";
        var request = new HttpRequestMessage(HttpMethod.Post, path)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(message),
                Encoding.UTF8,
                "application/json")
        };
        
        var response = await httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError($"Failed to publish message: {response.StatusCode}");
        }
        
        logger.LogInformation($"Published message: {message}");
    }

    public async Task<string> GetMessages()
    {
        string path = "messages";
        var request = new HttpRequestMessage(HttpMethod.Get, path);
        var response = await httpClient.SendAsync(request);
        var apiResponse = await response.Content.ReadAsStringAsync();
        return apiResponse;
    }
}