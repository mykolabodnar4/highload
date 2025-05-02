using System;
using Confluent.Kafka;
using MessagingService.Services;

namespace MessagingService.Extensions;

public static class Kafka
{
    public static IServiceCollection ConfigureKafkaConsumer(this IServiceCollection services, IConfiguration configuration) 
    {
        var bootstrapServers = configuration["Kafka:BootstrapServers"];
        if (string.IsNullOrEmpty(bootstrapServers))
        {
            throw new ArgumentException("Kafka:BootstrapServers configuration is missing or empty.");
        }

        services.AddSingleton<IConsumer<string, string>>(sp =>
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = "messaging-service-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                EnablePartitionEof = true,
            };

            return new ConsumerBuilder<string, string>(config)
            .SetErrorHandler((_, e) =>
            {
                Console.WriteLine($"Kafka error: {e.Reason}");
            })
            .Build();

        });

        services.AddHostedService<ConsumerService>();

        return services;
    }
}
