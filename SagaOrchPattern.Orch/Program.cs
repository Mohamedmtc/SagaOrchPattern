using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SagaOrchPattern.Orch
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
             .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
                 .ReadFrom.Configuration(hostingContext.Configuration)
                 .WriteTo.OpenTelemetry(options =>
                 {
                     var configuration = hostingContext.Configuration;
                     options.Endpoint = $"{configuration.GetValue<string>("Otlp:Endpoint")}/v1/logs";
                     options.Protocol = Serilog.Sinks.OpenTelemetry.OtlpProtocol.Grpc;
                     options.ResourceAttributes = new Dictionary<string, object>
                     {
                         ["service.name"] = configuration.GetValue<string>("Otlp:ServiceName")
                     };
                 }));
    }
}
