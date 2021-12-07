using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SagaOrchPattern.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using MassTransit;
using SagaOrchPattern.Core.RabbitMq.BusConfiguration;
using SagaOrchPattern.Orch.StateMachine.Order;
using MassTransit.EntityFrameworkCoreIntegration;
namespace SagaOrchPattern.Orch
{
    public class Startup
    {
        string connectionString = "Server=.;Database=OrchSagaDb;Trusted_Connection=True;";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<OrchSagaDbContext>((provider, builder) =>
            {
                builder.UseSqlServer(connectionString, m =>
                {
                    m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                    m.MigrationsHistoryTable($"__{nameof(OrchSagaDbContext)}");
                });
            });

            services.AddMassTransit(cfg =>
            {

                cfg.AddSagaStateMachine<OrderStateMachine, OrderStateData>()
                 .EntityFrameworkRepository(r =>
                 {
                     r.ConcurrencyMode = ConcurrencyMode.Pessimistic; // or use Optimistic, which requires RowVersion
                     r.ExistingDbContext<OrchSagaDbContext>();

                 });

                cfg.AddBus(provider => RabbitMqBus.ConfigureBusWebApi(Configuration, provider));
            });

            services.AddMassTransitHostedService();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
