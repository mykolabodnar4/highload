using FacadeService.Clients;
using FacadeService.Extensions;

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

        builder.Services.ConfigureGrpc(builder.Configuration);
        builder.Services.ConfigureKafkaProducer(builder.Configuration);
        
        builder.Services.AddHttpClient<MessagingApiClient>(client =>
        {
            var addresses = builder.Configuration.GetSection("MessagingService:Urls").Get<string[]>() ?? [];
            var index = new Random().Next(0, 1);
            client.BaseAddress = new Uri(addresses is [] ? "" : addresses[index]);
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
