using Hazelcast;

namespace LoggingService.Services;

public interface IHazelcastClientProvider
{
    Task<IHazelcastClient> GetHazelcastClient();
}
