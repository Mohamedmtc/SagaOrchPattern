using MassTransit;
using SagaOrchPattern.Messages.Order.Event;
using System.Threading.Tasks;

namespace SagaOrchPattern.Card.Consumer
{
    public class OrderStartedConsumer : IConsumer<IOrderStartedEvent>
    {
        public async Task Consume(ConsumeContext<IOrderStartedEvent> context)
        {

            await Task.CompletedTask;
        }
    }
}
