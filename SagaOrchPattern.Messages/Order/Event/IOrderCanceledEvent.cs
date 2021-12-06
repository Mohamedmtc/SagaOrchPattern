using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaOrchPattern.Messages.Order.Event
{
    public interface IOrderCanceledEvent
    {
        public Guid OrderId { get; set; }
        public string ExceptionMessage {  get; set; }
    }
}
