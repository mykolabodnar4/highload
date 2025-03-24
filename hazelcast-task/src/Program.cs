using Hazelcast;
using HazelcastTask;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class Program
{
    private static async Task Main(string[] args)
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        ILogger logger = factory.CreateLogger("Program");

        var options = new HazelcastOptionsBuilder()
            .WithDefault("Logging:LogLevel:Default", LogLevel.None)
            .WithDefault("Logging:LogLevel:Hazelcast", LogLevel.Information)
            .WithLoggerFactory(configuration => LoggerFactory.Create(builder => builder
                .AddConfiguration(configuration.GetSection("logging"))
                .AddSimpleConsole(consoleOptions =>
                {
                    consoleOptions.SingleLine = true;
                    consoleOptions.TimestampFormat = "hh:mm:ss.fff ";
                })))
            .With(config => 
            {
                config.Networking.Addresses.Clear();
                config.Networking.Addresses.Add("localhost:5701");
                config.Networking.Addresses.Add("localhost:5702");
                config.Networking.Addresses.Add("localhost:5703");
                config.ClusterName = "counter-cluster"; 
            })
            .Build();


        await using var client = await HazelcastClientFactory.StartNewClientAsync(options);
        

        // await MapWriter.Write1000KeysToMap(client, "1000_keys_map");

        var map = "counter";
        // var key = "counter-3-clients";
        // await MapWriter.WriteToKey(client, map, key);

        // var key2 = "counter-3-clients-pessimistic";
        // await MapWriter.WriteToKeyPessimistic(client, map, key2);

        // var key3 = "counter-3-clients-optimistic";
        // await MapWriter.WriteToKeyOptimistic(client, map, key3);

        var queueName = "test-queue";

        await (args[0] switch
        {
            "writer" => QueueWriter.WriteToQueue(client, queueName),
            "reader" => QueueReader.ReadFromQueue(client, queueName),
            _ => throw new NotSupportedException()
        });
    }
}