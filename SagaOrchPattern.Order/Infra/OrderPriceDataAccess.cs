using SagaOrchPattern.Order.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SagaOrchPattern.Order.Infra
{
    public class OrderPriceDataAccess : IOrderPriceDataAccess
    {
        public List<OrderPrice> GetAllOrder()
        {
            using (var context = new OrderDbContext())
            {
                return context.OrderPrices.ToList();
            }
        }
        public void SaveOrder(OrderPrice order)
        {
            using (var context = new OrderDbContext())
            {
                context.Add<OrderPrice>(order);
                context.SaveChanges();
            }
        }

        public bool DeleteOrder(Guid OrderId)
        {
            using (var context = new OrderDbContext())
            {
                OrderPrice order = context.OrderPrices.Where(x => x.OrderId == OrderId).FirstOrDefault();

                if (order != null)
                {
                    context.Remove(order);
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public OrderPrice GetOrder(Guid OrderId)
        {
            using (var context = new OrderDbContext())
            {
                return context.OrderPrices.Where(x => x.OrderId == OrderId).FirstOrDefault();
            }
        }

    }
}
