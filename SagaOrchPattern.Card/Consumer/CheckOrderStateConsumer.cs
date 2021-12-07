using MassTransit;
using SagaOrchPattern.Messages.Order.Command;
using SagaOrchPattern.Messages.Order.Event;
using System.Threading.Tasks;

namespace SagaOrchPattern.Card.Consumer
{
    public class CheckOrderStateConsumer : IConsumer<ICheckOrderStateCommand>
    {
        public async Task Consume(ConsumeContext<ICheckOrderStateCommand> context)
        {
  
            if(context.Message.IsCanceled)
            {
                await context.Publish<IOrderCanceledEvent>(new {
                    context.Message.OrderId,
                    ExceptionMessage = "order canceled"
                });
            }
            else
            {
                await context.Publish<IOrderFinishedEvent>(new
                {
                    context.Message.OrderId
                });
            }
            await Task.CompletedTask;
        }
    }
}
