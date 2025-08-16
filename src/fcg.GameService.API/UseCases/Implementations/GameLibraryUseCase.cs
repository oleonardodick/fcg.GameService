using fcg.GameService.API.DTOs.GameLibrary;
using fcg.GameService.API.DTOs.GameLibrary.Requests;
using fcg.GameService.API.DTOs.Responses;
using fcg.GameService.API.Entities;
using fcg.GameService.API.Mappers;
using fcg.GameService.API.Repositories.Interfaces;
using fcg.GameService.API.UseCases.Interfaces;

namespace fcg.GameService.API.UseCases.Implementations;

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

        return GameLibraryMapper.FromEntityToDto(result);
    }

    public async Task<ResponseGameLibraryDTO?> GetByUserIdAsync(string userId)
    {
        var result = await _repository.GetByUserIdAsync(userId);

        if (result is null)
            return null;

        return GameLibraryMapper.FromEntityToDto(result);

    }
    public async Task<ResponseGameLibraryDTO> CreateAsync(CreateGameLibraryDTO request)
    {
        var gameLibrary = GameLibraryMapper.FromDtoToEntity(request);

        await _repository.CreateAsync(gameLibrary);

        var response = GameLibraryMapper.FromEntityToDto(gameLibrary);

        return response;
    }

    public async Task<bool> AddGameToLibraryAsync(string libraryId, AddGameToLibraryDTO game)
    {
        if (await GetByIdAsync(libraryId) is null)
            return false;

        var gameAdquired = LibraryGamesMapper.FromDtoToEntity(game);

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
