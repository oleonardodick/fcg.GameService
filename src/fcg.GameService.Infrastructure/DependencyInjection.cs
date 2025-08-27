using fcg.GameService.Domain.Elasticsearch;
using fcg.GameService.Domain.Repositories;
using fcg.GameService.Infrastructure.Configurations;
using fcg.GameService.Infrastructure.Elasticsearch;
using fcg.GameService.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace fcg.GameService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        MongoDbSettings mongoDbSettings = new();
        configuration.GetSection(nameof(MongoDbSettings)).Bind(mongoDbSettings);

        services.AddHealthChecks()
            .AddMongoDb(
                mongodbConnectionString: mongoDbSettings!.ConnectionString,
                name: "mongodb",
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

            return new MongoClient(settings);
        });

        services.AddSingleton(sp =>
        {
            IMongoClient client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(mongoDbSettings.DatabaseName);
        });

        services.Configure<ElasticSettings>(configuration.GetSection(nameof(ElasticSettings)));
        services.AddSingleton<IElasticSettings>(sp => sp.GetRequiredService<IOptions<ElasticSettings>>().Value);

        services.AddSingleton(typeof(IElasticClient<>), typeof(ElasticClient<>));

        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IGameLibraryRepository, GameLibraryRepository>();

        return services;
    }
}
