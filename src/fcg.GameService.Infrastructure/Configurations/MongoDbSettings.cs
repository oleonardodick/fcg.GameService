using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;

namespace fcg.GameService.Infrastructure.Configurations;

public static class MongoDbSettings
{
    public static string ConnectionString { get; } =
        Environment.GetEnvironmentVariable("MongoDbSettings_ConnectionString")!;
    public static string DatabaseName { get; } =
        Environment.GetEnvironmentVariable("MongoDbSettings_DatabaseName")!;
}

public static class MongoDbService
{
    public static IServiceCollection AddMongoDBService(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddMongoDb(
                mongodbConnectionString: MongoDbSettings.ConnectionString,
                name: "mongodb",
                timeout: TimeSpan.FromSeconds(5),
                tags: ["db", "mongo"]
            );

        services.AddSingleton<IMongoClient>(sp =>
        {
            MongoClientSettings settings = MongoClientSettings.FromConnectionString(MongoDbSettings.ConnectionString);

            settings.ClusterConfigurator = cb =>
            {
                cb.Subscribe(new DiagnosticsActivityEventSubscriber());
            };

            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);

            settings.ConnectTimeout = TimeSpan.FromSeconds(5);

            return new MongoClient(settings);
        });

        services.AddSingleton(sp =>
        {
            IMongoClient client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(MongoDbSettings.DatabaseName);
        });

        return services;
    }
}
