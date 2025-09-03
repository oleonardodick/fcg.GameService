using fcg.GameService.Application.Interfaces;
using fcg.GameService.Application.Mappers.Adapters;
using fcg.GameService.Domain.Elasticsearch;
using fcg.GameService.Domain.Entities;
using fcg.GameService.Domain.Models;
using fcg.GameService.Domain.Repositories;
using fcg.GameService.Presentation.DTOs.Game.Requests;
using fcg.GameService.Presentation.DTOs.Game.Responses;

namespace fcg.GameService.Application.UseCases;

public class SuggestionUseCase(
    IGameRepository gameRepository,
    IGameLibraryRepository libraryRepository,
    IElasticClient<UserLog> elasticClientUser,
    IElasticClient<GameLog> elasticClientGame,
    IAppLogger<SuggestionUseCase> logger) : ISuggestionUseCase
{
    private readonly IElasticClient<UserLog> _elasticClientUser = elasticClientUser;
    private readonly IElasticClient<GameLog> _elasticClientGame = elasticClientGame;
    private readonly IGameRepository _gameRepository = gameRepository;
    private readonly IGameLibraryRepository _libraryRepository = libraryRepository;
    private readonly IAppLogger<SuggestionUseCase> _logger = logger;
    private const string LIBRARYENTITY = "Biblioteca";
    private const string GAMEENTITY = "Jogo";

    public async Task<IList<ResponseGameDTO>> GetSuggestionByUserIdAsync(SuggestGameDto request)
    {
        ElasticLogRequest elasticLogUserRequest = new(
            Index: LIBRARYENTITY,
            Field: "userId",
            Value: request.UserId,
            Page: request.Page,
            Size: request.Size);

        IReadOnlyCollection<UserLog> userLogs = await _elasticClientUser.Get(elasticLogUserRequest);

        if (userLogs.Count == 0)
        {
            _logger.LogWarning("Erro ao consultar a biblioteca no Elasticsearch");
            return [];
        }

        IEnumerable<string> tags = userLogs.SelectMany(ul => ul.Tags.Split('|')).Distinct();

        ElasticLogRequest elasticLogGameRequest = new(
            Index: GAMEENTITY,
            Field: "tags",
            Value: string.Join("|", tags),
            Page: request.Page,
            Size: request.Size);

        IReadOnlyCollection<GameLog> gameLogs = await _elasticClientGame.Get(elasticLogGameRequest);

        if (gameLogs.Count == 0)
        {
            _logger.LogWarning("Erro ao consultar a biblioteca no Elasticsearch");
            return [];
        }

        IEnumerable<string> taggedGames = gameLogs.Select(gl => gl.GameId).Distinct();

        GameLibrary? ownedGames = await _libraryRepository.GetByUserIdAsync(request.UserId);

        IEnumerable<string> suggestedGames = [];
        if (ownedGames != null || ownedGames?.Games.Count > 0)
            suggestedGames = taggedGames.Except(ownedGames.Games.Select(og => og.Id)).ToList();
        else
            suggestedGames = [.. taggedGames];

        IList<Game> response = await _gameRepository.GetAllByIdsAsync(suggestedGames);

        return GameMapperAdapter.FromListEntityToListDto(response);
    }
}
