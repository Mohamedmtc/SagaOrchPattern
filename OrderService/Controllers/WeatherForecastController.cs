using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SagaOrchPattern.Messages.Order.Event;
using System.Diagnostics;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IBus _bus;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IBus bus)
        {
            _logger = logger;
            _bus = bus;
        }

        [HttpGet("GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            var id = Guid.NewGuid();
            var traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            _logger.LogInformation($"TraceId: {traceId},Weather forecasts for {id}");
            _bus.Publish<IOrderStartedEvent>(new
            {
                OrderId = id,
                PaymentCardNumber = "number",
                ProductName = "tita",
                IsCanceled = false
            }).GetAwaiter().GetResult();

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = id.ToString()
            })
            .ToArray();
        }

        [HttpGet("MakeException")]
        public IEnumerable<WeatherForecast> MakeException()
        {
            var id = Guid.NewGuid();
            var traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            throw new Exception("atia error");

        }
    }
}
