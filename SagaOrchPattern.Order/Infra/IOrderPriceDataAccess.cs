using SagaOrchPattern.Order.Models;
using System;
using System.Collections.Generic;

namespace SagaOrchPattern.Order.Infra
{
    public interface IOrderPriceDataAccess
    {
        List<OrderPrice> GetAllOrder();

        void SaveOrder(OrderPrice order);

        OrderPrice GetOrder(Guid orderId);
        bool DeleteOrder(Guid orderId);
    }
}
