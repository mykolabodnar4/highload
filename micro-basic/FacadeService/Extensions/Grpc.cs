using System;
using FacadeApi.Grpc.Logging;
using FacadeService.Services;
using Grpc.Core;
using Grpc.Net.Client.Balancer;
using Grpc.Net.Client.Configuration;

namespace FacadeService.Extensions;

public static class Grpc
{
    public static IServiceCollection ConfigureGrpc(this IServiceCollection services, IConfiguration configuration)
    {
        var addresses = configuration.GetSection("LoggingService:Urls").Get<string[]>();
        if (addresses == null || addresses.Length == 0)
        {
            throw new ArgumentException("LoggingService:Urls configuration is missing or empty.");
        }
        var split = addresses
                .Select(address => (addr: address[..address.LastIndexOf(':')], port: int.Parse(address[(address.LastIndexOf(':') + 1)..]))).ToList();

        var balancerAddresses = split.Select(address => new BalancerAddress(address.addr, address.port)).ToList();
        var resolverFactory = new StaticResolverFactory(uri =>
        {
            Console.WriteLine($"ResolverFactory called for URI: {uri}");
            foreach (var address in balancerAddresses)
            {
                Console.WriteLine($"Resolved address: {address.EndPoint}");
            }
            return balancerAddresses;
        });
        services.AddSingleton<ResolverFactory>(_ => resolverFactory);
        services.AddSingleton<LoadBalancerFactory, RandomBalancerFactory>();

        services.AddGrpcClient<MessageLogger.MessageLoggerClient>(options =>
        {
            options.Address = new Uri("static://logging-service");
        })
        .ConfigurePrimaryHttpMessageHandler(() =>
        {
            var handler = new SocketsHttpHandler
            {
                SslOptions = new System.Net.Security.SslClientAuthenticationOptions
                {
                    RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
                }
            };
            return handler;
        })
        .ConfigureChannel(options =>
        {
            options.Credentials = ChannelCredentials.SecureSsl;
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
