using System;
using Hazelcast;

namespace MessagingService.Services;

public interface IHazelcastClientProvider
{
    Task<IHazelcastClient> GetHazelcastClient();
}
