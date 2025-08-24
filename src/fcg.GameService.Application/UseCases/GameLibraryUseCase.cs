using fcg.GameService.Application.Interfaces;
using fcg.GameService.Application.Mappers;
using fcg.GameService.Application.Mappers.Adapters;
using fcg.GameService.Domain.Repositories;
using fcg.GameService.Presentation.DTOs.GameLibrary.Requests;
using fcg.GameService.Presentation.DTOs.GameLibrary.Responses;

namespace fcg.GameService.Application.UseCases;

public class GameLibraryUseCase : IGameLibraryUseCase
{
    private readonly IGameLibraryRepository _repository;

    public GameLibraryUseCase(IGameLibraryRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResponseGameLibraryDTO?> GetByIdAsync(string id)
    {
        var result = await _repository.GetByIdAsync(id);

        if (result is null)
            return null;

        return GameLibraryMapperAdapter.FromEntityToDto(result);
    }

    public async Task<ResponseGameLibraryDTO?> GetByUserIdAsync(string userId)
    {
        var result = await _repository.GetByUserIdAsync(userId);

        if (result is null)
            return null;

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
        if (await GetByIdAsync(libraryId) is null)
            return false;

        var gameAdquired = GameLibraryMapperAdapter.FromGameAdquiredDtoToEntity(game);

        return await _repository.AddGameToLibraryAsync(libraryId, gameAdquired);
    }

    public async Task<bool> RemoveGameFromLibraryAsync(string libraryId, RemoveGameFromLibraryDTO game)
    {
        if (await GetByIdAsync(libraryId) is null)
            return false;

        return await _repository.RemoveGameFromLibraryAsync(libraryId, game.Id);
    }

    public async Task<bool> ExistsGameOnLibraryAsync(string libraryId, string gameId) =>
        await _repository.ExistsGameOnLibraryAsync(libraryId, gameId);
}
