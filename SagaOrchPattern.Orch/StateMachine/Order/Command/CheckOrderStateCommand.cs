﻿using SagaOrchPattern.DB;
using SagaOrchPattern.Messages.Order.Command;
using System;

namespace SagaOrchPattern.Orch.StateMachine.Order.Command
{
    public class CheckOrderStateCommand : ICheckOrderStateCommand
    {
        private readonly OrderStateData orderSagaState;
        public CheckOrderStateCommand(OrderStateData orderStateData)
        {
            this.orderSagaState = orderStateData;
        }
        public Guid OrderId => orderSagaState.OrderId;
        public string PaymentCardNumber => orderSagaState.PaymentCardNumber;
        public string ProductName => orderSagaState.ProductName;
        public bool IsCanceled => orderSagaState.IsCanceled;
    }
}
