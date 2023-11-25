using Serilog.Context;
using System.Diagnostics;

namespace OrderService
{
    public class TraceIdMiddleware
    {
        private readonly RequestDelegate _next;

        public TraceIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            var traceId = Activity.Current?.Id ?? Guid.NewGuid().ToString("N");
            context.Response.Headers.Add("TraceId", traceId);
            using (LogContext.PushProperty("TraceId", traceId))
            {
                await _next(context);
            }
            // Generate or retrieve the TraceId
            //var traceId = Activity.Current?.Id ?? Guid.NewGuid().ToString("N");

            //// Add TraceId to response headers
            //context.Response.Headers.Add("TraceId", traceId);

            // Include TraceId in logs (optional)
            // Use your preferred logging framework (e.g., Serilog, built-in logging)
            // logger.LogInformation($"TraceId: {traceId}, Request: {context.Request.Path}");

            // Continue processing the request pipeline
            //using (var activity = new Activity("RequestProcessing"))
            //{
            //    activity.AddTag("TraceId", traceId);
            //    activity.Start();

            //    await _next(context);

            //    activity.Stop();
            //}
        }
    }
}
