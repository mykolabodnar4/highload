using FacadeApi.Grpc.Logging;
using FacadeService.Clients;
using Grpc.Core;
using Grpc.Net.Client.Configuration;

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.ClearProviders().AddConsole();

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddGrpcClient<MessageLogger.MessageLoggerClient>(options =>
        {
            options.Address = new Uri(builder.Configuration["LoggingService:Url"] ?? string.Empty);
        }).ConfigureChannel(options =>
        {
            options.ServiceConfig = new ServiceConfig();
            options.ServiceConfig.MethodConfigs.Add(
                new MethodConfig
                {
                    Names = { MethodName.Default },
                    RetryPolicy = new RetryPolicy
                    {
                        MaxAttempts = 10,
                        InitialBackoff = TimeSpan.FromSeconds(1),
                        MaxBackoff = TimeSpan.FromSeconds(5),
                        BackoffMultiplier = 1.5,
                        RetryableStatusCodes = { StatusCode.Unavailable }
                    }
                }
            );
        });

        builder.Services.AddHttpClient<MessagingServiceClient>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["MessagingService:Url"] ?? string.Empty);
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.MapControllers();

        app.Run();
    }
}
