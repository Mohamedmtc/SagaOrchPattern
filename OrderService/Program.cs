using MassTransit.Logging;
using MassTransit;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using OrderService.RabbitMq.BusConfiguration;
using OrderService;
using System.Runtime.InteropServices;
using System.Reflection.PortableExecutable;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();


builder.Services.AddProblemDetails().AddExceptionHandler<GlobalExceptionHandler>();

#region MassTransit
builder.Services.AddMassTransit(cfg =>
{


    cfg.AddBus(provider => RabbitMqBus.ConfigureBusWebApi(builder.Configuration, provider));
});
builder.Services.AddMassTransitHostedService();

#endregion
var serviceName = builder.Configuration.GetValue<string>("OTEL:ServiceName");
string endPoint = builder.Configuration.GetValue<string>("OTEL:Endpoint");

#region Opentelemetry
Action<ResourceBuilder> appResourceBuilder =
resource => resource
.AddTelemetrySdk()
.AddService(serviceName: serviceName)
.AddAttributes(new Dictionary<string, object>
{
    ["host.name"] = Environment.MachineName,
    ["os.description"] = RuntimeInformation.OSDescription,
    ["deployment.environment"] = builder.Environment.EnvironmentName.ToLowerInvariant(),
}); ;


builder.Services.AddOpenTelemetry()
    .ConfigureResource(appResourceBuilder)
    .WithTracing(builder => builder
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddMassTransitInstrumentation()
        .AddSource(DiagnosticHeaders.DefaultListenerName)
        .AddSource("APITracing")
                .AddOtlpExporter(options => options.Endpoint = new Uri(uriString: endPoint))

    );
    //.WithMetrics(builder => builder
    //    .AddMeter("System.Net.Http")
    //    .AddMeter("Microsoft.AspNetCore.Hosting")
    //    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
    //    .AddMeter("Microsoft.AspNetCore.Http.Connections")

    //    .AddRuntimeInstrumentation()
    //    .AddHttpClientInstrumentation()
    //    .AddAspNetCoreInstrumentation()
    // .AddPrometheusExporter()
    //    .AddOtlpExporter(options => options.Endpoint = new Uri(uriString: endPoint))); 


//builder.Logging.AddOpenTelemetry(options =>
//{
//    options.AddOtlpExporter(options => options.Endpoint = new Uri(uriString: endPoint));
//    options.IncludeFormattedMessage = true;
//    options.IncludeScopes = true;
//    options.ParseStateValues = true;
//});
#endregion


var app = builder.Build();

app.UseCors(builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
});

app.UseStatusCodePages();

app.UseExceptionHandler();

app.UseMiddleware<TraceIdMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseHttpMetrics();
app.MapControllers();

app.UseMetricServer();
//app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.Run();
