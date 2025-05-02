namespace FacadeService.Clients;

public class MessagingApiClient(HttpClient httpClient, ILogger<MessagingApiClient> logger)
{
    public async Task<string> GetMessages()
    {
        try
        {
            string path = "messages";
            var request = new HttpRequestMessage(HttpMethod.Get, path);
            var response = await httpClient.SendAsync(request);
            var apiResponse = await response.Content.ReadAsStringAsync();
            return apiResponse;
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to get messages: {ex.Message}");
            return string.Empty;
        }
    }
}