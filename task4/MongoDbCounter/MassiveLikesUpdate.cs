using System;
using System.Diagnostics;
using MongoDB.Driver;

namespace MongoDbCounter;

public static class MassiveLikesUpdate
{
    public static async Task UpdateLikes(IMongoDatabase db, string collectionName, string counterName, WriteConcern writeConcern) {
        var collection = db.GetCollection<Counter>(collectionName).WithWriteConcern(writeConcern);

        var filter = Builders<Counter>.Filter.Eq(c => c.Name, counterName);
        var update = Builders<Counter>.Update.Inc(c => c.Value, 1);
        var options = new FindOneAndUpdateOptions<Counter>() { 
            ReturnDocument = ReturnDocument.After
        };

        Stopwatch stopwatch = new();
        var tasks = new List<Task>();
        stopwatch.Start();

        for(int i = 0; i < 10; i++) {
            int ii = i;
            var task = Task.Run(
            async () =>
            {
                for (int j = 0; j < 10_000; j++)
                {
                    try
                    {
                        var doc = await collection.FindOneAndUpdateAsync(filter, update, options);
                        Console.WriteLine($"Executed {j} of {ii}. Current value is {doc.Value}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception happened: {0}", e.Message);
                    }
                }
            });

            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
        stopwatch.Stop();
        Console.WriteLine("Update likes for an item in {0} ms", stopwatch.ElapsedMilliseconds);
    }
}
