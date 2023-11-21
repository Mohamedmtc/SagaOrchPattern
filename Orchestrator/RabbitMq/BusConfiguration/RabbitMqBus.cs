using GreenPipes;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Configuration;
using Orchestrator.RabbitMq.ApiConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.RabbitMq.BusConfiguration
{
    public static class RabbitMqBus
    {
        public static IBusControl ConfigureBusWebApi(IConfiguration configuration, IServiceProvider provider, Action<IRabbitMqBusFactoryConfigurator, IRabbitMqHost>
           registrationAction = null)
        {
            var HostName = @"rabbitmq://" + configuration.GetKeyValue<CONFIGKEYS.INTDV.SHARED.RibbitMQ>(key => key.Host) + @"/";
            var UserName = configuration.GetKeyValue<CONFIGKEYS.INTDV.SHARED.RibbitMQ>(key => key.Username);
            var Password = configuration.GetKeyValue<CONFIGKEYS.INTDV.SHARED.RibbitMQ>(key => key.Password);


            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.UseMessageRetry(r => { r.Immediate(5); });
                cfg.Host(new Uri(HostName), hst =>
                {
                    hst.Username(UserName);
                    hst.Password(Password);
                });

                cfg.ConfigureEndpoints((IBusRegistrationContext)provider);
            });
        }
    }
}
