using System.Diagnostics;
using Neo4j.Driver;

namespace GraphStore;

public static class MassiveLikesUpdate
{
    public static async Task UpdateLikes(IDriver driver, string itemId)
    {
        //await using var session = driver.AsyncSession(builder => builder.WithDatabase("neo4j").WithDefaultAccessMode(AccessMode.Write));
        await driver
            .ExecutableQuery("MATCH (item:Item {id:$itemId} ) SET item.likes = 0 FINISH")
            .WithParameters(new { itemId })
            .WithConfig(new QueryConfig(database: "neo4j"))
            .ExecuteAsync();
        var currentLikes = await GetItemLikes(driver, itemId);
        Console.WriteLine("Current number of likes: {0}", currentLikes);
        Stopwatch stopwatch = new Stopwatch();
        
        List<Task> tasks = new List<Task>();
        stopwatch.Start();
        for (int i = 0; i < 10; i++)
        {
            int ii = i;
            var task = Task.Run(
            async () =>
            {
                for (int j = 0; j < 10_000; j++)
                {
                    try
                    {
                        await using var session = driver.AsyncSession(builder => builder.WithDatabase("neo4j").WithDefaultAccessMode(AccessMode.Write));
                        var tx = await session.BeginTransactionAsync();
                        try
                        {
                            await tx.RunAsync(
                                "MATCH (item:Item {id:$itemId}) SET item.likes = item.likes + 1 RETURN item.likes as likes",
                                new { itemId });
                            await tx.CommitAsync();
                        }
                        catch (Exception e)
                        {
                            await tx.RollbackAsync();
                        }
                        //await session.RunAsync(
                        //    "MATCH (item:Item {id:$itemId}) SET item.likes = item.likes + 1 RETURN item.likes as likes",
                        //    new { itemId });
                        //Console.WriteLine($"Executed {j} of {ii}");
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

        //return await GetItemLikes(driver, itemId);
    }

    public static async Task<int> GetItemLikes(IDriver driver, string itemId)
    {
        var (likesQueryResult, _) = await driver
            .ExecutableQuery("MATCH (item:Item {id:$itemId}) RETURN item.likes as likes")
            .WithParameters(new { itemId })
            .WithConfig(new QueryConfig(database: "neo4j"))
            .ExecuteAsync();
        var likes = likesQueryResult.Single()["likes"].As<int>();
        return likes;
    }
    
}