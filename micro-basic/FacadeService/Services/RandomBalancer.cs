using Grpc.Net.Client.Balancer;

namespace FacadeService.Services;

public class RandomBalancer : SubchannelsLoadBalancer
{
    public RandomBalancer(IChannelControlHelper controller, ILoggerFactory loggerFactory)
        : base(controller, loggerFactory)
    {
    }

    protected override SubchannelPicker CreatePicker(IReadOnlyList<Subchannel> readySubchannels)
    {
        return new RandomPicker(readySubchannels.ToList());
    }
}

public class RandomPicker : SubchannelPicker
{
    private readonly List<Subchannel> _subchannels;

    public RandomPicker(List<Subchannel> subchannels)
    {
        _subchannels = subchannels;
    }

    public override PickResult Pick(PickContext context)
    {
        // Pick a random subchannel.
        return PickResult.ForSubchannel(_subchannels[Random.Shared.Next(0, _subchannels.Count)]);
    }
}
public class RandomBalancerFactory : LoadBalancerFactory
{
    // Create a RandomBalancer when the name is 'random'.
    public override string Name => "random";

    public override LoadBalancer Create(LoadBalancerOptions options)
    {
        return new RandomBalancer(options.Controller, options.LoggerFactory);
    }
}