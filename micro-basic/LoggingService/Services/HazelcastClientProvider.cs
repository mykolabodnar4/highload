using Hazelcast;
using Microsoft.Extensions.Options;

namespace LoggingService.Services;

public class HazelcastClientProvider(
    IOptions<HazelcastOptions> options
) : IHazelcastClientProvider, IAsyncDisposable
{
    private readonly HazelcastOptions _options = options.Value;
    private IHazelcastClient? _hazelcastClient;

    public async Task<IHazelcastClient> GetHazelcastClient()
    {
        return _hazelcastClient ??= await HazelcastClientFactory.StartNewClientAsync(_options);
    }

    public async ValueTask DisposeAsync()
    {
        if(_hazelcastClient is not null)
        {
            await _hazelcastClient.DisposeAsync();
        }
    }
}
