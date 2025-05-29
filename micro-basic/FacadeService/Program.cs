using Aoxe.Extensions.Configuration.Consul.Json;
using Consul;
using FacadeService.Clients;
using FacadeService.Extensions;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Steeltoe.Discovery.Consul;
using Steeltoe.Discovery.HttpClients;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.ClearProviders().AddConsole();
        builder.Configuration.AddConsulJson(
            new ConsulClientConfiguration
            {
                Address = new Uri("http://localhost:8500"),
                Datacenter = "dc1",
            },
            "facade-service"
        );

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddConsulDiscoveryClient();
        builder.Services.AddHttpClient<MessagingApiClient>(client =>
        {
            client.BaseAddress = new Uri("http://messaging-service/");
        }).AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(clientBuilder => clientBuilder.AddServiceDiscovery());
        builder.Services.ConfigureGrpc(builder.Configuration);
        builder.Services.ConfigureKafkaProducer(builder.Configuration);

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
