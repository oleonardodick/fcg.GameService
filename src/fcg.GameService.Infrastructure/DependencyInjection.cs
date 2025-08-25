using fcg.GameService.Domain.Repositories;
using fcg.GameService.Infrastructure.Configurations;
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
      
        var mongoDbSettings = new MongoDbSettings();
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
            var mongoDbSettings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;

            var settings = MongoClientSettings.FromConnectionString(mongoDbSettings.ConnectionString);

            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);

            settings.ConnectTimeout = TimeSpan.FromSeconds(5);

            return new MongoClient(settings);
        });

        services.AddSingleton(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(mongoDbSettings.DatabaseName);
        });

        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IGameLibraryRepository, GameLibraryRepository>();

        return services;
    }
}
