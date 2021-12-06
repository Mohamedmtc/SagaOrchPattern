using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaOrchPattern.Core.RabbitMq.ApiConfiguration
{
    public class CONFIGKEYS
    {
        public class INTDV
        {
            public class SHARED
            {
                public class RibbitMQ
                {
                    public string Host { get; }
                    public string Username { get; }
                    public string Password { get; }
                    public string ClientId { get; }
                }
            }
        }
    }
}
