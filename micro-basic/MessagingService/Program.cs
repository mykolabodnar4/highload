using Aoxe.Extensions.Configuration.Consul.Json;
using Consul;
using Hazelcast.DependencyInjection;
using MessagingService.Extensions;
using MessagingService.Services;
using Steeltoe.Discovery.Consul;

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Configuration.AddConsulJson(
            new ConsulClientConfiguration
            {
                Address = new Uri("http://localhost:8500"),
                Datacenter = "dc1",
            },
            "messaging-service"
        );

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddControllers();

        builder.Services.ConfigureKafkaConsumer(builder.Configuration);
        builder.Services.AddHazelcast(builder.Configuration);

        builder.Services.AddTransient<IMessageRepository, MessageRepository>();
        builder.Services.AddTransient<IHazelcastClientProvider, HazelcastClientProvider>();
        
        builder.Services.AddConsulDiscoveryClient();

        var app = builder.Build();
        // Configure the HTTP request pipeline.

        Console.WriteLine(app.Environment.EnvironmentName);

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapControllers();

        app.Run();
    }
}
