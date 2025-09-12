using fcg.GameService.Domain.Elasticsearch;
using fcg.GameService.Domain.Event;
using fcg.GameService.Domain.Repositories;
using fcg.GameService.Infrastructure.Configurations;
using fcg.GameService.Infrastructure.Elasticsearch;
using fcg.GameService.Infrastructure.Event;
using fcg.GameService.Infrastructure.Repositories;
using fcg.GameService.Presentation.Event.Consume;
using fcg.GameService.Presentation.Event.Publish;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace fcg.GameService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMongoDBService(configuration);

        services.Configure<ElasticSettings>(configuration.GetSection(nameof(ElasticSettings)));
        services.AddSingleton<IElasticSettings>(sp => sp.GetRequiredService<IOptions<ElasticSettings>>().Value);

        services.AddSingleton(typeof(IElasticClient<>), typeof(ElasticClient<>));

        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IGameLibraryRepository, GameLibraryRepository>();

        services.AddOpenTelemetrySettings(configuration);

        services.AddScoped<IConsumer<GamePurchaseConsumeEvent>, GamePurchaseConsumer>();
        services.AddScoped<IPublisher<GamePurchasePublishEvent>, GamePurchasePublisher>();

        return services;
    }
}
