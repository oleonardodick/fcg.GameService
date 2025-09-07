using fcg.GameService.Application.Interfaces;
using fcg.GameService.Infrastructure.Adapters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Enrichers.Span;

namespace fcg.GameService.Infrastructure.Configurations;

public static class LoggingSettings
{
    public static IServiceCollection AddLoggingConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        .Enrich.FromLogContext()
        .Enrich.WithSpan()
        .WriteTo.Console()
        .WriteTo.OpenTelemetry()
        .CreateLogger();

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(Log.Logger, dispose: true);
        });

        services.AddSingleton(typeof(IAppLogger<>), typeof(MicrosoftLogAdapter<>));

        return services;
    }
}
