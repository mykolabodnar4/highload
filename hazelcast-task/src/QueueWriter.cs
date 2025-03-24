using System;
using Hazelcast;

namespace HazelcastTask;

public class QueueWriter
{
    public static async Task WriteToQueue(IHazelcastClient client, string queueName)
    {
        await using var queue = await client.GetQueueAsync<string>(queueName);
 
        var producerTask = Task.Run(async () =>
        {
            for(int i = 0; i < 100; i++)
            {
                Console.WriteLine($"Write message: {i}");
                
                bool enqueued = await queue.OfferAsync(i.ToString());
                Console.WriteLine($"Item enqueued");
                Thread.Sleep(100);
            }
        });

        await producerTask;
    }
}
