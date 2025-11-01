using fcg.GameService.Application.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;

namespace fcg.GameService.Infrastructure.HealthChecks;

public class MongoDbHealthCheck : IHealthCheck
{
    private readonly IMongoClient _mongoClient;
    private readonly IAppLogger<MongoDbHealthCheck> _logger;
        public MongoDbHealthCheck(IMongoClient mongoClient, IAppLogger<MongoDbHealthCheck> logger)
    {
        _mongoClient = mongoClient ?? throw new ArgumentNullException(nameof(mongoClient));
        _logger = logger;
    }
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await _mongoClient.ListDatabaseNamesAsync(cancellationToken);
            
            return HealthCheckResult.Healthy("Sucesso ao conectar no MongoDB");
        }
        catch (TimeoutException ex)
        {
            var message = "A aplicação apresentou um timeout ao tentar conectar ao MongoDB";
            _logger.LogWarning(message);
            return HealthCheckResult.Degraded(message, ex);
        }
        catch (Exception ex)
        {
            var message = "Falha ao tentar conectar ao MongoDB";
            _logger.LogError(ex, message);
            return HealthCheckResult.Unhealthy(message, ex);
        }
    }
}
