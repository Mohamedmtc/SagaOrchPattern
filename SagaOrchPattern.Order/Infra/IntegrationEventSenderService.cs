using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using MassTransit;
using System.Reflection;
using Newtonsoft.Json;

namespace SagaOrchPattern.Order.Infra
{
    public class IntegrationEventSenderService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IBus _bus;
        public IntegrationEventSenderService(IServiceScopeFactory scopeFactory, IBus bus)
        {
            _scopeFactory = scopeFactory;
            using var scope = _scopeFactory.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
            dbContext.Database.EnsureCreated();
            _bus = bus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await PublishOutstandingIntegrationEvents(stoppingToken);
            }
        }

        private async Task PublishOutstandingIntegrationEvents(CancellationToken stoppingToken)
        {
            try
            {


                while (!stoppingToken.IsCancellationRequested)
                {
                    {

                        using var scope = _scopeFactory.CreateScope();
                        using var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
                        var events = dbContext.OutBoxs.OrderBy(o => o.OutBoxId).ToList();
                        foreach (var e in events)
                        {
                            var MessageType = Type.GetType(e.Event);
                            var messageBody = JsonConvert.DeserializeObject(e.Message, MessageType);
                            await _bus.Publish(messageBody, stoppingToken);
                            Console.WriteLine("Published: " + e.Event + " " + e.Message);
                            dbContext.OutBoxs.Remove(e);
                            dbContext.SaveChanges();
                        }
                    }
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
