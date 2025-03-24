using System;
using System.Diagnostics;
using Hazelcast;

namespace HazelcastTask;

public class MapWriter
{
    public async static Task Write1000KeysToMap(IHazelcastClient client, string mapName)
    {
        await using var map = await client.GetMapAsync<int, int>(mapName);

        Stopwatch stopwatch = Stopwatch.StartNew();

        for (int i = 1; i <= 1000; i++)
        {
            await map.SetAsync(i, i * 10);
        }

        stopwatch.Stop();
        Console.WriteLine($"Finished writing to '{mapName}' in {stopwatch.ElapsedMilliseconds}");
    }

    public async static Task WriteToKey(IHazelcastClient client, string mapName, string key)
    {
        await using var map = await client.GetMapAsync<string, int>(mapName);
        await map.PutIfAbsentAsync(key, 0);

        Stopwatch stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < 10_000; i++)
        {
            var counterValue = await map.GetAsync(key);
            counterValue++;
            await map.PutAsync(key, counterValue);
        }

        stopwatch.Stop();

        Console.WriteLine($"Finished writing to '{mapName}' in {stopwatch.ElapsedMilliseconds}");
    }

    public async static Task WriteToKeyPessimistic(IHazelcastClient client, string mapName, string key)
    {
        await using var map = await client.GetMapAsync<string, int>(mapName);
        await map.PutIfAbsentAsync(key, 0);

        Stopwatch stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < 10_000; i++)
        {
            await map.LockAsync(key);

            var counterValue = await map.GetAsync(key);
            //Thread.Sleep(5);

            counterValue++;
            await map.SetAsync(key, counterValue);
            await map.UnlockAsync(key);
        }

        stopwatch.Stop();

        Console.WriteLine($"Finished writing to '{mapName}' in {stopwatch.ElapsedMilliseconds}");
    }

    public async static Task WriteToKeyOptimistic(IHazelcastClient client, string mapName, string key)
    {
        await using var map = await client.GetMapAsync<string, int>(mapName);
        await map.PutIfAbsentAsync(key, 0);

        Stopwatch stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < 10_000; i++)
        {
            while (true)
            {
                var counterValue = await map.GetAsync(key);
                var newValue = counterValue + 1;
                if(await map.ReplaceAsync(key, counterValue, newValue)) 
                {
                    break;
                }
            }
        }

        stopwatch.Stop();

        Console.WriteLine($"Finished writing to '{mapName}' in {stopwatch.ElapsedMilliseconds}");
    }
}
