using Microsoft.AspNetCore.Mvc;

namespace SimpleApi;

[ApiController]
[Route("weatherforecast")]
public class WeatherForecastController : ControllerBase
{
    private static List<string> summaries = [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    [HttpGet]
    public IActionResult Get()
    {
        var forecast = Enumerable.Range(0, 5).Select(index =>
                        new WeatherForecast
                        (
                            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                            Random.Shared.Next(-21, 55),
                            summaries[Random.Shared.Next(summaries.Count)]
                        ))
                        .ToArray();
        return Ok(forecast);
    }

    [HttpPost]
    public IActionResult Post([FromBody] Summary summary)
    {
        summaries.Add(summary.Name);
        return Ok(summaries.IndexOf(summary.Name));
    }
}

public record Summary(string Name);
