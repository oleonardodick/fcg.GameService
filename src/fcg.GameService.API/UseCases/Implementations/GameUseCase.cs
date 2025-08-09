using fcg.GameService.API.DTOs.Requests;
using fcg.GameService.API.DTOs.Responses;
using fcg.GameService.API.Entities;
using fcg.GameService.API.Exceptions;
using fcg.GameService.API.Helpers;
using fcg.GameService.API.Repositories.Interfaces;
using fcg.GameService.API.UseCases.Interfaces;

namespace fcg.GameService.API.UseCases.Implementations;

public class GameUseCase : IGameUseCase
{
    private readonly IGameRepository _repository;

    public GameUseCase(IGameRepository repository)
    {
        _repository = repository;
    }

    public async Task<IList<ResponseGameDTO>> GetAllAsync()
    {
        var games = await _repository.GetAllAsync();

        var response = games.Select(game => new ResponseGameDTO
        {
            Id = game.Id,
            Name = game.Name,
            Description = game.Description ?? "",
            Price = game.Price,
            ReleasedDate = game.ReleasedDate,
            Tags = game.Tags,
        }).ToList();

        return response;
    }

    public async Task<ResponseGameDTO?> GetByIdAsync(string id)
    {
        var game = await _repository.GetByIdAsync(id);

        if (game == null)
            throw new NotFoundException(
                "Jogo não encontrado",
                $"Não foi encontrado o jogo com o ID {id}"
            );

        var response = new ResponseGameDTO
        {
            Id = game.Id,
            Name = game.Name,
            Description = game.Description ?? "",
            Price = game.Price,
            ReleasedDate = game.ReleasedDate,
            Tags = game.Tags,
        };
        return response;
    }

    public async Task<ResponseGameDTO> CreateAsync(CreateGameDTO request)
    {
        var game = new Game {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            ReleasedDate = request.ReleasedDate,
            Tags = TagHelper.NormalizeTags(request.Tags)
        };

        await _repository.CreateAsync(game);

        var response = new ResponseGameDTO
        {
            Id = game.Id,
            Name = game.Name,
            Description = game.Description ?? string.Empty,
            Price = game.Price,
            ReleasedDate = game.ReleasedDate,
            Tags = game.Tags
        };

        return response;
    }

    public async Task<bool> UpdateAsync(string id, UpdateGameDTO request)
    {
        var gameToUpdate = await GetByIdAsync(id);

        if (gameToUpdate is null)
            return false;

        var tags = TagHelper.NormalizeTags(request.Tags ?? gameToUpdate.Tags);

        var game = new Game
        {
            Id = id,
            Name = request.Name ?? gameToUpdate.Name,
            Description = request.Description ?? gameToUpdate.Description,
            Price = request.Price ?? gameToUpdate.Price,
            ReleasedDate = request.ReleasedDate ?? gameToUpdate.ReleasedDate,
            Tags = tags
        };

        await _repository.UpdateAsync(game);

        return true;
    }

    public async Task<bool> UpdateTagsAsync(string id, string[] tags)
    {
        var gameToUpdate = await GetByIdAsync(id);

        if (gameToUpdate is null)
            return false;

        tags = TagHelper.NormalizeTags(tags);

        await _repository.UpdateTagsAsync(id, tags);
        return true;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var gameToDelete = await GetByIdAsync(id);

        if (gameToDelete is null)
            return false;

        await _repository.DeleteAsync(id);
        return true;
    }
}
