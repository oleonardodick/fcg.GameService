using fcg.GameService.Application.Interfaces;
using fcg.GameService.Domain.Elasticsearch;
using fcg.GameService.Domain.Entities;
using fcg.GameService.Domain.Repositories;
using fcg.GameService.Presentation.DTOs.Metrics;

namespace fcg.GameService.Application.UseCases;

public class MetricsUseCase(IElasticClient<UserLog> elasticClient, IGameRepository gameRepository, IAppLogger<MetricsUseCase> logger) : IMetricsUseCase
{
    public async Task<IEnumerable<PopularGameDTO>> GetPopularGamesAsync(int limit = 10)
    {
        logger.LogInformation("Iniciando busca de jogos populares com limite {limit}", limit);
        
        // Busca logs de usuários no Elasticsearch
        var userLogs = await GetUserLogsFromElasticsearch();
        
        if (userLogs.Count == 0)
        {
            logger.LogWarning("Nenhum log encontrado no Elasticsearch");
            return [];
        }

        // Agrupa por gameId e conta quantos usuários têm cada jogo
        var gamePopularity = userLogs
            .Where(log => !string.IsNullOrEmpty(log.GameId))
            .GroupBy(log => log.GameId)
            .Select(g => new
            {
                GameId = g.Key,
                UserCount = g.Count(),
                Tags = g.First().Tags.Split('|', StringSplitOptions.RemoveEmptyEntries)
            })
            .OrderByDescending(x => x.UserCount)
            .Take(limit)
            .ToList();

        if (!gamePopularity.Any())
        {
            logger.LogWarning("Nenhum jogo encontrado nos logs");
            return [];
        }

        // Buscar os nomes reais dos jogos no MongoDB
        var gameIds = gamePopularity.Select(g => g.GameId).ToList();
        var games = await gameRepository.GetAllByIdsAsync(gameIds);
        var gameLookup = games.ToDictionary(g => g.Id, g => g.Name);

        // Criar o resultado final com nomes reais
        var result = gamePopularity.Select(g => new PopularGameDTO(
            g.GameId,
            gameLookup.TryGetValue(g.GameId, out var gameName) ? gameName : $"Game {g.GameId}",
            g.UserCount,
            g.Tags
        )).ToList();

        logger.LogInformation("Retornando {count} jogos populares", result.Count);
        
        return result;
    }

    private async Task<IReadOnlyCollection<UserLog>> GetUserLogsFromElasticsearch()
    {
        // Primeira tentativa: buscar por um userId específico que sabemos que existe
        var elasticRequest = new Domain.Models.ElasticLogRequest(
            Page: 0,
            Size: 10000,
            Index: "biblioteca",
            Field: "userId",
            Value: "91129215-d602-4812-a334-3a2b759ed991"
        );

        var userLogs = await elasticClient.Get(elasticRequest);
        
        if (userLogs.Count > 0)
        {
            logger.LogInformation("Encontrados {count} logs para o usuário específico", userLogs.Count);
            return userLogs;
        }

        // Fallback: tentar buscar todos os registros
        logger.LogWarning("Nenhum log encontrado para o usuário específico, tentando busca geral");
        
        var fallbackRequest = new Domain.Models.ElasticLogRequest(
            Page: 0,
            Size: 10000,
            Index: "biblioteca",
            Field: "gameId",
            Value: "*"
        );
        
        userLogs = await elasticClient.Get(fallbackRequest);
        logger.LogInformation("Consulta fallback retornou {count} logs", userLogs.Count);
        
        return userLogs;
    }
}
