using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Timeout;
using ReplicatedLog;
using ReplicatedLog.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddCommandLine(args);
builder.Logging.AddSimpleConsole(options =>
{
    options.IncludeScopes = true;
    options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss]";
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.
AddHttpClient<IReplicationService, ReplicationService>(client =>
{
    client.Timeout = TimeSpan.FromHours(1);
})
.AddResilienceHandler("replication", pipeline =>
{
    pipeline.AddRetry(new Polly.Retry.RetryStrategyOptions<HttpResponseMessage>
    {
        MaxRetryAttempts = int.MaxValue,
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true,
        ShouldHandle = new PredicateBuilder<HttpResponseMessage>() // Defines exceptions to trigger retries
                    .Handle<HttpRequestException>() // Includes any HttpRequestException
                    .Handle<TimeoutRejectedException>() // Includes any TimeoutRejectedException
                    .HandleResult(response => !response.IsSuccessStatusCode)
                    
    });
    pipeline.AddTimeout(TimeSpan.FromMilliseconds(1000));
});

builder.Services
    .AddSingleton<GlobalSequencer>()
    .AddSingleton<SortedDictionary<int, Message>>()
    .AddSingleton<ConcurrentDictionary<string, Message>>()
    .AddTransient<ILogRepository, InMemoryLogRepository>()
    .Configure<LogConfig>(builder.Configuration.GetSection("LogConfig"));

builder.Services.AddTransient<ILogService>(provider =>
{
    var logConfig = provider.GetRequiredService<IOptions<LogConfig>>().Value;
    if (logConfig.Mode == AppMode.Secondary)
    {
        return new SimpleLogService(provider.GetRequiredService<ILogRepository>());
    }
    else
    {
        return new ReplicatedLogService(
            provider.GetRequiredService<ILogRepository>(),
            provider.GetRequiredService<IReplicationService>());
    }
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
