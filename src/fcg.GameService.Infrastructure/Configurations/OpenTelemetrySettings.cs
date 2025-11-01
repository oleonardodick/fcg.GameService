using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
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
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.Filter = (httpContext) =>
                        {
                            /* Filtra para não gerar tracing do health check*/
                            var path = httpContext.Request.Path.Value ?? string.Empty;
                            return !path.Contains("/health", StringComparison.OrdinalIgnoreCase);
                        };
                    })
                    .AddHttpClientInstrumentation(opt =>
                    {
                        opt.FilterHttpRequestMessage = (httpRequest) =>
                        {
                            /* Filtra para não gerar tracing do SQS, pois o sistema
                            ficará realizando POSTS sucessivos, o que impossibilitaria a
                            avaliação dos outros traces caso necessário.
                            Também remove trace do swagger, pois isso não é necessário
                            */
                            var url = httpRequest.RequestUri?.AbsoluteUri ?? string.Empty;
                            return !(url.Contains("sqs", StringComparison.OrdinalIgnoreCase)
                                || url.Contains("sns", StringComparison.OrdinalIgnoreCase)
                                || url.Contains("localstack", StringComparison.OrdinalIgnoreCase)
                                || url.Contains("host.docker.internal", StringComparison.OrdinalIgnoreCase)
                                || url.Contains(":4566", StringComparison.OrdinalIgnoreCase)
                                || url.Contains("swagger", StringComparison.OrdinalIgnoreCase));
                        };
                    })
                    .AddMongoDBInstrumentation();
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();
            })
            .WithLogging()
            .UseOtlpExporter();
            
        return services;
    }
}
