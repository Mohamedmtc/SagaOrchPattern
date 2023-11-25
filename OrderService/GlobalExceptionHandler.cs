﻿using MassTransit;
using Microsoft.AspNetCore.Diagnostics;
using System.Diagnostics;

namespace OrderService
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

            logger.LogError(
                exception,
                "Could not process a request on machine {MachineName}. TraceId: {TraceId}",
                Environment.MachineName,
                traceId
            );
            httpContext.Response.Headers.Add("TraceId", traceId);
            var (statusCode, title) = MapException(exception);

            await Results.Problem(
                title: title,
                statusCode: statusCode,
                extensions: new Dictionary<string, object?>
                {
                {"traceId",  traceId}
                }
            ).ExecuteAsync(httpContext);

            return true;
        }
        private static (int StatusCode, string Title) MapException(Exception exception)
        {
            return exception switch
            {
                ArgumentOutOfRangeException => (StatusCodes.Status400BadRequest, exception.Message),
                _ => (StatusCodes.Status500InternalServerError, "We made a mistake but we are on it!")
            };
        }

    }

}
