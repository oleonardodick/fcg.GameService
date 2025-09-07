using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace fcg.GameService.Infrastructure.Configurations;

public static class OpenTelemetrySettings
{
    public static IServiceCollection AddOpenTelemetrySettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resource =>
                resource
                    .AddTelemetrySdk()
                    .AddEnvironmentVariableDetector()
            )
            .WithTracing(tracing =>
            {
                tracing
                    .AddSource("MongoDB.Driver.Core.Extensions.DiagnosticSources")
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.Filter = (httpContext) =>
                        {
                            return !httpContext.Request.Path.Value?.Contains("/health") ?? true;
                        };
                    })
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter();
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter();
            })
            .WithLogging(logging =>
            {
                logging.AddOtlpExporter();
            });
            
        return services;
    }
}
