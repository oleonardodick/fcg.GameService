using fcg.GameService.Application.Interfaces;
using fcg.GameService.Application.Mappers.Adapters;
using fcg.GameService.Domain.Exceptions;
using fcg.GameService.Domain.Repositories;
using fcg.GameService.Presentation.DTOs.GameLibrary.Requests;
using fcg.GameService.Presentation.DTOs.GameLibrary.Responses;

namespace fcg.GameService.Application.UseCases;

public class GameLibraryUseCase : IGameLibraryUseCase
{
    private readonly IGameLibraryRepository _repository;
    private const string ENTITY = "Biblioteca";

    public GameLibraryUseCase(IGameLibraryRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResponseGameLibraryDTO?> GetByIdAsync(string id)
    {
        var result = await _repository.GetByIdAsync(id);

        if (result is null)
            throw AppNotFoundException.ForEntity(ENTITY, id);

        return GameLibraryMapperAdapter.FromEntityToDto(result);
    }

    public async Task<ResponseGameLibraryDTO?> GetByUserIdAsync(string userId)
    {
        var result = await _repository.GetByUserIdAsync(userId);

        if (result is null)
            throw new AppNotFoundException($"{ENTITY} não encontrada para o usuário {userId}");

        return GameLibraryMapperAdapter.FromEntityToDto(result);

    }
    public async Task<ResponseGameLibraryDTO> CreateAsync(CreateGameLibraryDTO request)
    {
        var gameLibrary = GameLibraryMapperAdapter.FromDtoToEntity(request);

        var createdLibrary = await _repository.CreateAsync(gameLibrary);

        var response = GameLibraryMapperAdapter.FromEntityToDto(createdLibrary);

        return response;
    }

    public async Task<bool> AddGameToLibraryAsync(string libraryId, AddGameToLibraryDTO game)
    {
        await GetByIdAsync(libraryId);

        var gameAdquired = GameLibraryMapperAdapter.FromGameAdquiredDtoToEntity(game);

        return await _repository.AddGameToLibraryAsync(libraryId, gameAdquired);
    }

    public async Task<bool> RemoveGameFromLibraryAsync(string libraryId, RemoveGameFromLibraryDTO game)
    {
        await GetByIdAsync(libraryId);

        return await _repository.RemoveGameFromLibraryAsync(libraryId, game.Id);
    }

    public async Task<bool> ExistsGameOnLibraryAsync(string libraryId, string gameId) =>
        await _repository.ExistsGameOnLibraryAsync(libraryId, gameId);
}
