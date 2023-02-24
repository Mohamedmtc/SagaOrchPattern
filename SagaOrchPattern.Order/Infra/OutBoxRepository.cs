using SagaOrchPattern.Order.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SagaOrchPattern.Order.Infra
{
    public class OutBoxRepository : IOutBoxRepository
    {
        private readonly OrderDbContext _orderDbContext;

        public OutBoxRepository(OrderDbContext orderDbContext)
        {
            _orderDbContext = orderDbContext;
        }

        public bool DeleteOutBox(Guid outboxId)
        {
            _orderDbContext.Remove(outboxId);
            _orderDbContext.SaveChanges();  
            return true;
        }

        public List<OutBox> GetAllOutBoxs()
        {
            return _orderDbContext.OutBoxs.ToList();
        }
    }
}
