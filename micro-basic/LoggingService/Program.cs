using System.Collections.Concurrent;
using LoggingApi.Grpc;
using Hazelcast;
using Hazelcast.DependencyInjection;
using LoggingService.Services;
using LoggingService;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Logging.ClearProviders().AddConsole();

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        var services = builder.Services; 
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddGrpc();

        services.AddSerilog(options => {
            options.WriteTo.Console(new RenderedCompactJsonFormatter(), LogEventLevel.Information);
            options.Enrich.FromLogContext();
            options.Enrich.WithMachineName();
        });

        builder.Services.AddHazelcast(builder.Configuration);
        builder.Services.Configure<ApplicationSettings>(
            builder.Configuration.GetSection(ApplicationSettings.Application));
        services.AddScoped<ILogRepository, HazelcastLogRepository>();
        services.AddScoped<IHazelcastClientProvider, HazelcastClientProvider>();
        
        var app = builder.Build();

        app.UseSerilogRequestLogging(options =>
        {
            // Customize the message template
            options.MessageTemplate = "Handled {RequestPath}";
            
            // Emit debug-level events instead of the defaults
            options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Debug;
            
            // Attach additional properties to the request completion event
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            };
        });

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
