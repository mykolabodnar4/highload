using System;
using Hazelcast;

namespace HazelcastTask;

public class QueueReader
{
    public static async Task ReadFromQueue(IHazelcastClient client, string queueName)
    {
        await using var queue = await client.GetQueueAsync<string>(queueName);

        var consumerTask = Task.Run(async () => {
            var consumed = 0;
            while(consumed < 100)
            {
                // PollAsync doesn't wait for a message to appear
                string? message = await queue.PollAsync();
                if(message is null)
                {
                    continue;
                }
                Console.WriteLine($"Consumed item: {message}");
            }
        });

        await consumerTask;
    }
}
