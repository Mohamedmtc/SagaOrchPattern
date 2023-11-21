using Automatonymous;
using Orchestrator.StateMachine.Order.Command;
using SagaOrchPattern.DB;
using SagaOrchPattern.Messages.Order.Event;

using System;

namespace Orchestrator.StateMachine.Order
{
    public class OrderStateMachine : MassTransitStateMachine<OrderStateData>
    {
        public State OrderStarted { get; private set; }
        public State OrderCancelled { get; private set; }
        public State OrderFinished { get; private set; }

        public Event<IOrderStartedEvent> OrderStartedEvent { get; private set; }
        public Event<IOrderCanceledEvent> OrderCancelledEvent { get; private set; }
        public Event<IOrderFinishedEvent> OrderFinishedEvent { get; private set; }

        public OrderStateMachine()
        {
            Event(() => OrderStartedEvent, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => OrderCancelledEvent, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => OrderFinishedEvent, x => x.CorrelateById(m => m.Message.OrderId));
            InstanceState(x => x.CurrentState);

            Initially(
               When(OrderStartedEvent)
                .Then(context =>
                {
                    context.Instance.OrderCreationDateTime = DateTime.Now;
                    context.Instance.OrderId = context.Data.OrderId;
                    context.Instance.PaymentCardNumber = context.Data.PaymentCardNumber;
                    context.Instance.ProductName = context.Data.ProductName;
                    context.Instance.IsCanceled = context.Data.IsCanceled;
                })
               .TransitionTo(OrderStarted)
                .Publish(context => new CheckOrderStateCommand(context.Instance)));


            During(OrderStarted,
               When(OrderCancelledEvent)
                   .Then(context =>
                   {
                       context.Instance.Exception = context.Data.ExceptionMessage;
                       context.Instance.OrderCancelDateTime = DateTime.Now;
                   })
                    .TransitionTo(OrderCancelled));


            During(OrderStarted,
                When(OrderFinishedEvent)
                    .Then(context =>
                    {
                        context.Instance.OrderFinishedDateTime = DateTime.Now;
                    })
                     .TransitionTo(OrderFinished)
                     .Finalize());

            SetCompletedWhenFinalized();
        }
    }
}
