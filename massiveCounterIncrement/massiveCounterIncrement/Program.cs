using Hazelcast;
using Hazelcast.Core;
using Hazelcast.CP;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Threading;


// create options
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
        config.Networking.Addresses.Add("host.docker.internal:5701");
        config.Networking.Addresses.Add("host.docker.internal:5702");
        config.Networking.Addresses.Add("host.docker.internal:5703");
        config.ClusterName = "counter-cluster";
    })
    .Build();



// create and connect a Hazelcast client to a server running on localhost
await using var client = await HazelcastClientFactory.StartNewClientAsync(options);


async Task CounterNonBlocking( IHazelcastClient hazelcastClient )
{
    var map = await hazelcastClient.GetMapAsync<string, int>("map-counter");
    await map.SetAsync("counter", 0);
    var value = await map.GetAsync("counter");

    List<Task> tasks = new List<Task>();
    for (int i = 0; i < 10; i++)
    {
        var task = Task.Run(
            async () =>
            {
                for (int j = 0; j < 10000; j++)
                {
                    var value = await map.GetAsync("counter");
                    value++;
                    await map.SetAsync("counter", value);
                }
            }
            );
        tasks.Add(task);
    }

    await Task.WhenAll(tasks);
    var valueCouter = await map.GetAsync("counter");
    Console.WriteLine("CounterNonBlocking " + valueCouter);
    await map.DisposeAsync();
}
//Stopwatch sw1 = Stopwatch.StartNew();
//await CounterNonBlocking(client);
//sw1.Stop();
//Console.WriteLine("Time taken: {0}ms", sw1.Elapsed.TotalMilliseconds);

async Task CounterPesemisticBlocking(IHazelcastClient hazelcastClient)
{
    await using var map = await hazelcastClient.GetMapAsync<string, int>("map");
    const string key = "counter";
    await map.SetAsync(key, 0);
    List<Task> tasks = new List<Task>();
    for (int i = 0; i < 10; i++)
    {
        using (AsyncContext.New())
        { 
            var task = Task.Run(
                async () =>
                {
                    for (int j = 0; j < 100; j++)
                    {
                        await map.LockAsync(key);

                        try
                        {
                            var value = await map.GetAsync(key);
                            await map.SetAsync(key, ++value);
                        }
                        finally
                        {  
                            await map.UnlockAsync(key);
                        }
                    }
                }
                );
            tasks.Add(task);
        }
    }
    await Task.WhenAll(tasks);
    var valueCouter = await map.GetAsync(key);
    Console.WriteLine("CounterPesemisticBlocking " + valueCouter);
    await map.DisposeAsync();
}

//Stopwatch sw2 = Stopwatch.StartNew();
//await CounterPesemisticBlocking(client);
//sw2.Stop();
//Console.WriteLine("Time taken: {0}ms", sw2.Elapsed.TotalMilliseconds);

async Task CounterOptimisticBlocking(IHazelcastClient hazelcastClient)
{
    var map = await hazelcastClient.GetMapAsync<string, int>("map-counter");
    await map.SetAsync("counter", 0);

    List<Task> tasks = new List<Task>();
    for (int i = 0; i < 10; i++)
    {
        var task = Task.Run(async () => {
            for (int j = 0; j < 10000; j++)
            {
                while (true)
                {
                    var counterValue = await map.GetAsync("counter");

                    var newValue = counterValue + 1;
                    var hasReplaced = await map.ReplaceAsync("counter", counterValue, newValue);
                    if (hasReplaced)
                    {
                        break;
                    }
                }
            }
        });
        tasks.Add(task);
    }

    await Task.WhenAll(tasks);
    var valueCouter = await map.GetAsync("counter");
    Console.WriteLine("CounterOptimisticBlocking " + valueCouter);
    await map.DisposeAsync();
}

//Stopwatch sw3 = Stopwatch.StartNew();
//await CounterOptimisticBlocking(client);
//sw3.Stop();
//Console.WriteLine("Time taken: {0}ms", sw3.Elapsed.TotalMilliseconds);

async Task CounterAtomicLong(IHazelcastClient hazelcastClient)
{

    await using var counter = await client.CPSubsystem.GetAtomicLongAsync("counterCP");

    List<Task> tasks = new List<Task>();
    for (int i = 0; i < 10; i++)
    {
        var task = Task.Run(
            async () =>
            {
                for (int j = 0; j < 10000; j++)
                {
                    await counter.IncrementAndGetAsync();
                }
            }
            );
        tasks.Add(task);
    }

    await Task.WhenAll(tasks);
    Console.WriteLine($"Count is {await counter.GetAsync()}");
}

//Stopwatch sw4 = Stopwatch.StartNew();
//await CounterAtomicLong(client);
//sw4.Stop();
//Console.WriteLine("Time taken: {0}ms", sw4.Elapsed.TotalMilliseconds);



// the client is disposed and thus disconnected on exit
