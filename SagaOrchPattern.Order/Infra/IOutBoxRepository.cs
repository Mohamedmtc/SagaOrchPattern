using SagaOrchPattern.Order.Models;
using System.Collections.Generic;
using System;

namespace SagaOrchPattern.Order.Infra
{
    public interface IOutBoxRepository
    {
        List<OutBox> GetAllOutBoxs();
        bool DeleteOutBox(Guid outboxId);
    }
}
