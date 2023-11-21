using MassTransit;
using OpenTelemetry.Trace;
using SagaOrchPattern.Messages.Order.Command;
using SagaOrchPattern.Messages.Order.Event;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CardService.Consumer
{
    public class CheckOrderStateConsumer : IConsumer<ICheckOrderStateCommand>
    {
        private readonly ILogger<CheckOrderStateConsumer> _logger;
        private readonly TracerProvider _tracerProvider;
        public CheckOrderStateConsumer(ILogger<CheckOrderStateConsumer> logger, TracerProvider tracerProvider)
        {
            _logger = logger;
            _tracerProvider = tracerProvider;
        }

        public async Task Consume(ConsumeContext<ICheckOrderStateCommand> context)
        {
            _logger.LogInformation("Check Order State details");


            try
            {
                if (context.Message.IsCanceled)
                {
                    await context.Publish<IOrderCanceledEvent>(new
                    {
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
                throw new InvalidOperationException();
            }
            catch (Exception ex)
            {
                // Capture and log the exception details including the source code line
                var exceptionDetails = new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                _logger.LogError(ex, "An exception occurred . Details: {@ExceptionDetails}", exceptionDetails);

                // Additionally, capture the exception details in the OpenTelemetry span
                var span = _tracerProvider.GetTracer("http-client")
                    .StartActiveSpan("request");

                span.SetAttribute("exception.message", ex.Message);
                span.SetAttribute("exception.stackTrace", ex.StackTrace);

                // Rethrow the exception if needed
                throw;
            }



            await Task.CompletedTask;
        }
    }
}
