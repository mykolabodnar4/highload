using System;
using FacadeApi.Grpc.Logging;
using FacadeService.Services;
using Grpc.Core;
using Grpc.Net.Client.Balancer;
using Grpc.Net.Client.Configuration;
using Steeltoe.Discovery.HttpClients;

namespace FacadeService.Extensions;

public static class Grpc
{
    public static IServiceCollection ConfigureGrpc(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ResolverFactory, ConsulResolverFactory>();
        services.AddSingleton<LoadBalancerFactory, RandomBalancerFactory>();

        services.AddGrpcClient<MessageLogger.MessageLoggerClient>(options =>
        {
            options.Address = new Uri("consul://logging-service");
        })

        .ConfigureChannel(options =>
        {
            options.Credentials = ChannelCredentials.Insecure;
            options.ServiceConfig = new ServiceConfig();
            options.ServiceConfig.LoadBalancingConfigs.Clear();
            options.ServiceConfig.LoadBalancingConfigs.Add(new LoadBalancingConfig("random"));
            
            options.ServiceProvider = services.BuildServiceProvider();
            options.ServiceConfig.MethodConfigs.Add(
                new MethodConfig
                {
                    Names = { MethodName.Default },
                    RetryPolicy = new RetryPolicy
                    {
                        MaxAttempts = 5,
                        InitialBackoff = TimeSpan.FromSeconds(1),
                        MaxBackoff = TimeSpan.FromSeconds(5),
                        BackoffMultiplier = 1.5,
                        RetryableStatusCodes = { StatusCode.Unavailable }
                    }
                }
            );
        });

        return services;
    }
}
