using fcg.GameService.Application.Interfaces;
using fcg.GameService.Application.Mappers.Adapters;
using fcg.GameService.Domain.Elasticsearch;
using fcg.GameService.Domain.Entities;
using fcg.GameService.Domain.Exceptions;
using fcg.GameService.Domain.Repositories;
using fcg.GameService.Presentation.DTOs.GameLibrary.Requests;
using fcg.GameService.Presentation.DTOs.GameLibrary.Responses;

namespace fcg.GameService.Application.UseCases;

public class GameLibraryUseCase(
    IGameLibraryRepository repository,
    IElasticClient<UserLog> elasticClient) : IGameLibraryUseCase
{
    private readonly IElasticClient<UserLog> _elasticClient = elasticClient;
    private readonly IGameLibraryRepository _repository = repository;
    private const string ENTITY = "Biblioteca";

    public async Task<ResponseGameLibraryDTO?> GetByIdAsync(string id)
    {
        GameLibrary? result = await _repository.GetByIdAsync(id) ??
            throw AppNotFoundException.ForEntity(ENTITY, id);
        return GameLibraryMapperAdapter.FromEntityToDto(result);
    }

    public async Task<ResponseGameLibraryDTO?> GetByUserIdAsync(string userId)
    {
        GameLibrary? result = await _repository.GetByUserIdAsync(userId) ??
            throw new AppNotFoundException($"{ENTITY} não encontrada para o usuário {userId}");
        return GameLibraryMapperAdapter.FromEntityToDto(result);

    }

    public async Task<ResponseGameLibraryDTO> CreateAsync(CreateGameLibraryDTO request)
    {
        GameLibrary gameLibrary = GameLibraryMapperAdapter.FromDtoToEntity(request);

        GameLibrary createdLibrary = await _repository.CreateAsync(gameLibrary);

        await _elasticClient.AddOrUpdate(new UserLog
            (createdLibrary.UserId,
             string.Join("|", createdLibrary.Games.Select(g => g.Tags))
            ), ENTITY);

        ResponseGameLibraryDTO response = GameLibraryMapperAdapter.FromEntityToDto(createdLibrary);

        return response;
    }

    public async Task<bool> AddGameToLibraryAsync(string libraryId, AddGameToLibraryDTO game)
    {
        ResponseGameLibraryDTO? library = await GetByIdAsync(libraryId);

        GameAdquired gameAdquired = GameLibraryMapperAdapter.FromGameAdquiredDtoToEntity(game);

        bool addedGame = await _repository.AddGameToLibraryAsync(libraryId, gameAdquired);

        if (addedGame)
            await _elasticClient.AddOrUpdate(new UserLog
                (library!.UserId,
                 string.Join("|", gameAdquired.Tags)
                ), ENTITY);

        return addedGame;
    }

    public async Task<bool> RemoveGameFromLibraryAsync(string libraryId, RemoveGameFromLibraryDTO game)
    {
        await GetByIdAsync(libraryId);

        return await _repository.RemoveGameFromLibraryAsync(libraryId, game.Id);
    }

    public async Task<bool> ExistsGameOnLibraryAsync(string libraryId, string gameId) =>
        await _repository.ExistsGameOnLibraryAsync(libraryId, gameId);
}
