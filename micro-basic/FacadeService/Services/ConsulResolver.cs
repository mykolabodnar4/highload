using System;
using Grpc.Net.Client.Balancer;
using Steeltoe.Common.Discovery;

namespace FacadeService.Services;

public class ConsulResolver(
    string serviceName,
    ILoggerFactory loggerFactory,
    IEnumerable<IDiscoveryClient> discoveryClients
) : PollingResolver(loggerFactory)
{

    protected override async Task ResolveAsync(CancellationToken cancellationToken)
    {
        var uris = await ResolveAsync(discoveryClients);
        var addresses = uris.Select(uri => new BalancerAddress(uri.Host, uri.Port)).ToArray();

        Listener(ResolverResult.ForResult(addresses));
    }

    private async Task<Uri[]?> ResolveAsync(IEnumerable<IDiscoveryClient> clients)
    {
        foreach (var client in clients)
        {
            var instances = await client.GetInstancesAsync(serviceName, default);
            return instances.Select(x => x.Uri).ToArray();
        }
        return null;
    }
}

public class ConsulResolverFactory(
    IEnumerable<IDiscoveryClient> discoveryClients
) : ResolverFactory
{
    // Create a ConsulResolver when the URI has a 'consul' scheme.
    public override string Name => "consul";

    public override Resolver Create(ResolverOptions options)
    {
        return new ConsulResolver(options.Address.Host, options.LoggerFactory, discoveryClients);
    }
}