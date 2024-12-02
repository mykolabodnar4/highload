using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDbCounter;

var mongoConnectionString = "mongodb://127.0.0.1:27017,127.0.0.1:27018,127.0.0.1:27019/?replicaSet=rs0";
var routine = args[0];

switch (routine)
{
    case "no-timeout-test":
        await NoTimeoutW3Test(mongoConnectionString);
        break;
    case "timeout-test":
        await WithTimeoutW3Test(mongoConnectionString);
        break;
    case "reelection-test":
        await PrimaryReelectionTest(mongoConnectionString);
        break;
    case "massive-update-test":
        await MassiveUpdate(mongoConnectionString);
        break;
    default:
        break;
}

async Task MassiveUpdate(string connectionString)
{
    var client = new MongoClient(connectionString);
    var db = client.GetDatabase("counters");

    var counters = db.GetCollection<Counter>("counters");
    
    counters.InsertOne(new Counter("likes", 0));

    await MassiveLikesUpdate.UpdateLikes(db, "counters", "likes", WriteConcern.W1);

    var likesW1 = await counters.AsQueryable().FirstOrDefaultAsync(x => x.Name == "likes");
    Console.WriteLine($"Current number of likes is: {likesW1.Value}");

    Console.WriteLine("Press c to continue and any other key to stop");
    var key = Console.ReadKey();

    if (key.KeyChar != 'c')
    {
        Console.WriteLine($"{key.KeyChar} pressed. Exiting application.");
        Thread.Sleep(2000);
        Environment.Exit(0);
    }

    Console.WriteLine("Continue");

    likesW1.Value = 0;
    await counters.FindOneAndReplaceAsync(x => x.Name == "likes", likesW1);
    await MassiveLikesUpdate.UpdateLikes(db, "counters", "likes", WriteConcern.WMajority);

    var likesMajority = await counters.AsQueryable().FirstOrDefaultAsync(x => x.Name == "likes");
    Console.WriteLine($"Current number of likes is: {likesMajority.Value}");
}


async Task NoTimeoutW3Test(string connectionString)
{
    Console.WriteLine(nameof(NoTimeoutW3Test));
    var noTimeoutConnectionSettings = MongoClientSettings.FromConnectionString(connectionString);
    noTimeoutConnectionSettings.ConnectTimeout = TimeSpan.FromDays(1);
    noTimeoutConnectionSettings.SocketTimeout = TimeSpan.FromDays(1);
    using var clientWithNoTimeout = new MongoClient(noTimeoutConnectionSettings);

    var db1 = clientWithNoTimeout.GetDatabase("counters");
    Console.WriteLine($"Connected to db counters");

    var countersW3 = db1.GetCollection<Counter>("counters");
    await countersW3.WithWriteConcern(WriteConcern.W3)
        .InsertOneAsync(new Counter("w3-counter", 1), new InsertOneOptions() { });
}

async Task WithTimeoutW3Test(string connectionString)
{
    Console.WriteLine(nameof(WithTimeoutW3Test));
    var timeoutConnectionSettings = MongoClientSettings.FromConnectionString(connectionString);
    timeoutConnectionSettings.ConnectTimeout = TimeSpan.FromMilliseconds(1000);
    timeoutConnectionSettings.SocketTimeout = TimeSpan.FromMilliseconds(1000);
    using var clientWithNoTimeout = new MongoClient(timeoutConnectionSettings);
    var db1 = clientWithNoTimeout.GetDatabase("counters");

    var countersW3 = db1.GetCollection<Counter>("counters");

    try
    {
        Console.WriteLine($"Writing to counters collection");
        await countersW3.WithWriteConcern(WriteConcern.W3).InsertOneAsync(new Counter("w3-15sec-counter", 1));
    }
    catch (MongoConnectionException e)
    {
        Console.WriteLine(e.Message);
    }

    Console.WriteLine("Trying to read the counter w3-15sec-counter value");
    var counterValue = countersW3.WithReadConcern(ReadConcern.Majority).AsQueryable()
        .First(x => x.Name == "w3-15sec-counter");
    Console.WriteLine($"Current counter value is: {counterValue.Value}");
}

async Task PrimaryReelectionTest(string connectionString)
{
    using var client = new MongoClient(connectionString);
    var db = client.GetDatabase("counters");

    var counters = db.GetCollection<Counter>("counters").WithWriteConcern(WriteConcern.W2);
    
    await counters.InsertOneAsync(new Counter("reelection-counter", 1));

    for (int i = 0; i < 15000; i++)
    {
        var filter = Builders<Counter>.Filter.Eq(c => c.Name, "reelection-counter");
        var update = Builders<Counter>.Update.Inc(c => c.Value, 1);
        var options = new FindOneAndUpdateOptions<Counter>() { ReturnDocument = ReturnDocument.After };
        var updatedDoc = await counters.FindOneAndUpdateAsync(filter, update, options);
        Console.WriteLine($"Current value is {updatedDoc.Value}");
        Thread.Sleep(100);
    }
}