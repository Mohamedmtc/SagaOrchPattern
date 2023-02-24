using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SagaOrchPattern.Core.RabbitMq.BusConfiguration;
using SagaOrchPattern.Order.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SagaOrchPattern.Order
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        { 
            ////////////// Configure Masstransit //////////////////////
            services.AddMassTransit(cfg =>
            {
                cfg.AddBus(provider => RabbitMqBus.ConfigureBusWebApi(Configuration, provider));
            });
            services.AddMassTransitHostedService();
            ////////////////////////////////////////


            ////////////////////////////////// Database and Access layer //////////////////////
            services.AddDbContext<OrderDbContext> (o => o.UseSqlServer(Configuration.GetConnectionString("OrderingDatabase")));
            services.AddScoped<OrderDbContext>();
            services.AddSingleton<IOrderPriceDataAccess, OrderPriceDataAccess>();
            services.AddScoped<IOutBoxRepository, OutBoxRepository>();
            services.AddSingleton<IntegrationEventSenderService>();
            services.AddHostedService<IntegrationEventSenderService>(provider => provider.GetService<IntegrationEventSenderService>());
            ////////////////////////////////////////

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
