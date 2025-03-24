using System.Collections.Concurrent;
using LoggingApi.Grpc;

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Logging.ClearProviders().AddConsole();

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddGrpc();

        builder.Services.AddSingleton(new ConcurrentDictionary<string, Message>());
        
        
        var services = builder.Services;
        services.AddScoped<ILogRepository, InMemoryLogRepository>();
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.MapGrpcService<MessageLoggerService>();
        app.Run();
    }
}
