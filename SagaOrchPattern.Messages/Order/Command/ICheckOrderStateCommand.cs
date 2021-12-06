using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaOrchPattern.Messages.Order.Command
{
    public interface ICheckOrderStateCommand
    {
        public Guid OrderId { get; set; }

        public string PaymentCardNumber { get; set; }

        public string ProductName { get; set; }
    }
}
