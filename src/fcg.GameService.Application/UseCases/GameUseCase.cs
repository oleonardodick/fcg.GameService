using fcg.GameService.Application.Helpers;
using fcg.GameService.Application.Interfaces;
using fcg.GameService.Application.Mappers.Adapters;
using fcg.GameService.Domain.Elasticsearch;
using fcg.GameService.Domain.Entities;
using fcg.GameService.Domain.Exceptions;
using fcg.GameService.Domain.Repositories;
using fcg.GameService.Presentation.DTOs.Game.Requests;
using fcg.GameService.Presentation.DTOs.Game.Responses;

namespace fcg.GameService.Application.UseCases;

public class GameUseCase(
    IGameRepository repository,
    IElasticClient<GameLog> elasticClient,
    IAppLogger<GameUseCase> logger) : IGameUseCase
{
    private readonly IElasticClient<GameLog> _elasticClient = elasticClient;
    private readonly IGameRepository _repository = repository;
    private readonly IAppLogger<GameUseCase> _logger = logger;
    private const string ENTITY = "Jogo";

    public async Task<IList<ResponseGameDTO>> GetAllAsync()
    {
        IList<Game> games = await _repository.GetAllAsync();

        IList<ResponseGameDTO> response = GameMapperAdapter.FromListEntityToListDto(games);

        return response;
    }

    public async Task<ResponseGameDTO?> GetByIdAsync(string id)
    {
        Game? game = await _repository.GetByIdAsync(id) ??
            throw AppNotFoundException.ForEntity(ENTITY, id);
        ResponseGameDTO response = GameMapperAdapter.FromEntityToDto(game);

        return response;
    }

    public async Task<ResponseGameDTO> CreateAsync(CreateGameDTO request)
    {
        Game game = GameMapperAdapter.FromDtoToEntity(request);

        Game createdGame = await _repository.CreateAsync(game);

        bool elastic = await _elasticClient.AddOrUpdate(new GameLog
            (createdGame.Id,
             string.Join("|", createdGame.Tags)
            ), ENTITY);

        if (!elastic)
            _logger.LogWarning("Erro ao indexar a biblioteca no Elasticsearch");

        ResponseGameDTO response = GameMapperAdapter.FromEntityToDto(createdGame);

        return response;
    }

    public async Task<bool> UpdateAsync(string id, UpdateGameDTO request)
    {
        Game? gameToUpdate = await _repository.GetByIdAsync(id);

        if (request.Tags is not null)
            request.Tags = TagHelper.NormalizeTags(request.Tags);

        gameToUpdate!.Update(
            request.Name,
            request.Price,
            request.ReleasedDate,
            request.Tags,
            request.Description
        );

        bool updated = await _repository.UpdateAsync(gameToUpdate!);

        bool elastic = await _elasticClient.AddOrUpdate(new GameLog
            (gameToUpdate.Id,
             string.Join("|", gameToUpdate.Tags)
            ), ENTITY);

        if (!elastic)
            _logger.LogWarning("Erro ao indexar a biblioteca no Elasticsearch");

        return updated;
    }

    public async Task<bool> UpdateTagsAsync(string id, UpdateTagsDTO tags)
    {
        await GetByIdAsync(id);

        tags.Tags = TagHelper.NormalizeTags(tags.Tags);

        return await _repository.UpdateTagsAsync(id, tags.Tags);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        await GetByIdAsync(id);

        return await _repository.DeleteAsync(id);
    }
}
