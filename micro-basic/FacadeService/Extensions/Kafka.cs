using System.Net;
using Confluent.Kafka;
using FacadeService.Services;

namespace FacadeService.Extensions;

public static class Kafka
{
    public static IServiceCollection ConfigureKafkaProducer(this IServiceCollection services, IConfiguration configuration) 
    {
        var bootstrapServers = configuration["Kafka:BootstrapServers"];
        if (string.IsNullOrEmpty(bootstrapServers))
        {
            throw new ArgumentException("Kafka:BootstrapServers configuration is missing or empty.");
        }

        services.AddSingleton<IProducer<string, string>>(sp =>
        {
            var config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
                ClientId = Dns.GetHostName(),
                EnableIdempotence = true,
                Acks = Acks.All,
                CompressionType = CompressionType.Gzip,
                LingerMs = 5,
                BatchSize = 32 * 1024, // 32 KB
                MaxInFlight = 5,
                SocketTimeoutMs = 10000,
                MessageTimeoutMs = 30000,
            };

            return new ProducerBuilder<string, string>(config).Build();
        });
        
        services.AddTransient<IKafkaPublisher, KafkaPublisher>();

        return services;
    }
}
