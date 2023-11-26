using MassTransit.Logging;
using MassTransit;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using OrderService.RabbitMq.BusConfiguration;
using OrderService;

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

#region Serilog
string servicename = builder.Configuration.GetValue<string>("Otlp:ServiceName");
string otlpENdPoint = builder.Configuration.GetValue<string>("Otlp:Endpoint");
builder.Host.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(hostingContext.Configuration)
    .WriteTo.OpenTelemetry(options =>
    {
        options.Endpoint = $"{otlpENdPoint}/v1/logs";
        options.Protocol = Serilog.Sinks.OpenTelemetry.OtlpProtocol.GrpcProtobuf;
        options.ResourceAttributes = new Dictionary<string, object>
        {
            ["service.name"] = servicename
        };
    }));
#endregion

#region Opentelemetry
Action<ResourceBuilder> appResourceBuilder =
resource => resource
.AddTelemetrySdk()
.AddService(builder.Configuration.GetValue<string>("Otlp:ServiceName"));

builder.Services.AddOpenTelemetry()
    .ConfigureResource(appResourceBuilder)
    .WithTracing(builder => builder
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddMassTransitInstrumentation()
        .AddSource(DiagnosticHeaders.DefaultListenerName)
        .AddSource("APITracing")
        //.AddConsoleExporter()
        .AddOtlpExporter(options => options.Endpoint = new Uri(otlpENdPoint))
    )
    .WithMetrics(builder => builder
        .AddRuntimeInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddOtlpExporter(options => options.Endpoint = new Uri(otlpENdPoint))); 
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

app.MapControllers();

app.Run();
