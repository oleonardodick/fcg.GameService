using fcg.GameService.Infrastructure.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;

namespace fcg.GameService.Infrastructure.Configurations;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}

public static class MongoDbService
{
    public static IServiceCollection AddMongoDBService(this IServiceCollection services, IConfiguration configuration)
    {
        MongoDbSettings mongoDbSettings = new();
        configuration.GetSection(nameof(MongoDbSettings)).Bind(mongoDbSettings);

        services.AddHealthChecks()
            .AddCheck<MongoDbHealthCheck>(
                name: "mongodb",
                failureStatus: HealthStatus.Unhealthy,
                timeout: TimeSpan.FromSeconds(5),
                tags: ["db", "mongo"]
            );

        services.Configure<MongoDbSettings>(
            configuration.GetSection(nameof(MongoDbSettings))
        );

        services.AddSingleton<IMongoClient>(sp =>
        {
            MongoDbSettings mongoDbSettings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;

            MongoClientSettings settings = MongoClientSettings.FromConnectionString(mongoDbSettings.ConnectionString);

            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);

            settings.ConnectTimeout = TimeSpan.FromSeconds(5);

            settings.ClusterConfigurator = cb => cb.Subscribe(new DiagnosticsActivityEventSubscriber());

            return new MongoClient(settings);
        });

        services.AddSingleton(sp =>
        {
            IMongoClient client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(mongoDbSettings.DatabaseName);
        });

        return services;
    }
}
