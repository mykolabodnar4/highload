
using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Numerics;
using System.Reflection.Emit;

var connectionString = "Host=localhost;Port=5432;User ID=postgres;Password=password;Database=postgres";
await using var dataSource = NpgsqlDataSource.Create(connectionString);

await using var connection = await dataSource.OpenConnectionAsync();

string dropTable = "DROP TABLE IF EXISTS user_counter";

await connection.ExecuteAsync(dropTable);

string createTable = """
    CREATE TABLE user_counter (
      userId SERIAL PRIMARY KEY,
      counter INTEGER,
      version INTEGER
    );
    """;

await connection.ExecuteAsync(createTable);

string createUser = """
    INSERT INTO user_counter (counter, version)
    VALUES (0, 1);
    """;

await connection.ExecuteAsync(createUser);

string selectUserCounter = """SELECT counter FROM user_counter WHERE userId = 1""";

async Task LostUpdate(NpgsqlDataSource npgsqlDataSource)
{
    string updateUserCounter = "update user_counter set counter = @counter where userId = 1";
    Stopwatch sw = Stopwatch.StartNew();

    List<Task> tasks = new List<Task>();
    for (int i = 0; i < 10; i++)
    {

        var connection = dataSource.CreateConnection();

        var task = Task.Run(
            async () =>
            {
                connection.Open();
                for (int j = 0; j < 10000; j++)
                {
                    var counter = await connection.QueryFirstAsync<int>(selectUserCounter);
                    counter++;
                    var parameters = new { counter };
                    await connection.ExecuteAsync(updateUserCounter, parameters);
                }
                connection.Close();
            }
            );
        tasks.Add(task);
    }

    await Task.WhenAll(tasks);
    sw.Stop();
    Console.WriteLine("Time taken: {0}ms", sw.Elapsed.TotalMilliseconds);
}

//await LostUpdate(dataSource);

async Task InPlaceUpdate(NpgsqlDataSource npgsqlDataSource)
{
    string updateUserCounter = "update user_counter set counter = counter + 1 where userId = 1";
    Stopwatch sw = Stopwatch.StartNew();

    List<Task> tasks = new List<Task>();
    for (int i = 0; i < 10; i++)
    {

        var connection = dataSource.CreateConnection();

        var task = Task.Run(
            async () =>
            {
                connection.Open();
                for (int j = 0; j < 10000; j++)
                {
                    await connection.ExecuteAsync(updateUserCounter);
                }
                connection.Close();
            }
            );
        tasks.Add(task);
    }

    await Task.WhenAll(tasks);
    sw.Stop();
    Console.WriteLine("Time taken: {0}ms", sw.Elapsed.TotalMilliseconds);

}

//await InPlaceUpdate(dataSource);



async Task RowLevelLocking(String connectionString)
{
    string updateUserCounter = "update user_counter set counter = @counter where userId = 1";
    string selectUserCounterRowLevelLocking = """SELECT counter FROM user_counter WHERE userId = 1 FOR UPDATE """;
    Stopwatch sw = Stopwatch.StartNew();
    List<Task> tasks = new List<Task>();

    for (int i = 0; i < 10; i++)
    {
        var connection = new NpgsqlConnection(connectionString);
        var task = Task.Run(async () =>
            {

                    connection.Open();
                    for (int j = 0; j < 10000; j++)
                     {
                    var transaction = connection.BeginTransaction();
                    var counter = await connection.QueryFirstAsync<int>(selectUserCounterRowLevelLocking);
                        counter++;
                        var parameters = new { counter };
                        await connection.ExecuteAsync(updateUserCounter, parameters);
                    transaction.Commit();
                }

                    connection.Close();
                });
                tasks.Add(task); 
         
    }

    await Task.WhenAll(tasks);
    sw.Stop();
    Console.WriteLine("Time taken: {0}ms", sw.Elapsed.TotalMilliseconds);
}

await RowLevelLocking(connectionString);

async Task Optimistic(NpgsqlDataSource npgsqlDataSource)
{
    string updateUserCounter = "update user_counter set counter = @counter,version = @versionPlusOne where userId = 1 and version = @version";
    string selectUserCounterOptimistic = "SELECT counter, version FROM user_counter WHERE userId = 1";
    Stopwatch sw = Stopwatch.StartNew();

    List<Task> tasks = new List<Task>();
    for (int i = 0; i < 10; i++)
    {

        var connection = dataSource.CreateConnection();

        var task = Task.Run(
            async () =>
            {
                connection.Open();
                for (int j = 0; j < 10000; j++)
                {
                    while (true)
                    {
                        var (counter, version) = await connection.QueryFirstAsync<(int,int)>(selectUserCounterOptimistic);
                        counter++;
                        var parameters = new { counter = counter, version = version, versionPlusOne = version + 1 };
                        var count = await connection.ExecuteAsync(updateUserCounter, parameters);

                        if (count > 0) break;
                    }
                   
                }
                connection.Close();
            }
            );
        tasks.Add(task);
    }

    await Task.WhenAll(tasks);
    sw.Stop();
    Console.WriteLine("Time taken: {0}ms", sw.Elapsed.TotalMilliseconds);
}

//await Optimistic(dataSource);

var counter = await connection.QueryFirstAsync<int>(selectUserCounter);
Console.WriteLine($"Count is {counter}");


