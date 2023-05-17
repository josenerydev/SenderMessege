using Azure.Storage.Queues;

using Microsoft.AspNetCore.Mvc;

using System.Text.Json;

namespace SenderMessege.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly string _connectionString;
        private readonly string _queueName;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("MyStorageConnection");
            _queueName = configuration.GetValue<string>("QueueName");
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var weatherForecast = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

            QueueClientOptions options = new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            };

            QueueClient queue = new QueueClient(_connectionString, _queueName, options);

            await queue.CreateAsync();

            foreach (var forecast in weatherForecast)
            {
                // Serialize the forecast to a JSON string
                string message = JsonSerializer.Serialize(forecast);

                // Send the message to the queue
                await queue.SendMessageAsync(message);
            }

            return weatherForecast;
        }
    }
}