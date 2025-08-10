using fcg.GameService.API.DTOs.GameLibrary;
using fcg.GameService.API.DTOs.GameLibrary.Requests;
using fcg.GameService.API.DTOs.Responses;
using fcg.GameService.API.Entities;
using fcg.GameService.API.Exceptions;
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

    public async Task<ResponseGameLibraryDTO> GetByUserIdAsync(string userId)
    {
        var result = await _repository.GetByUserIdAsync(userId);

        if (result == null)
            throw new NotFoundException(
                "Livraria de Jogos não encontrada",
                $"Não foi encontrada a livraria de jogos do usuário {userId}"
            );

        return new ResponseGameLibraryDTO
        {
            Id = result.Id,
            UserId = result.UserId,
            Games = result.Games.Select(gameAdquired => new ResponseGameAdquiredDTO
            {
                Id = gameAdquired.Id,
                Name = gameAdquired.Name
            }).ToList()
        };
    }
    public async Task<ResponseGameLibraryDTO> CreateAsync(CreateGameLibraryDTO request)
    {
        var gameLibrary = new GameLibrary
        {
            UserId = request.userId
        };

        await _repository.CreateAsync(gameLibrary);

        var response = new ResponseGameLibraryDTO
        {
            Id = gameLibrary.Id,
            UserId = gameLibrary.UserId,
        };

        return response;
    }

    public async Task<bool> AddGameToLibraryAsync(AddGameToLibraryDTO game)
    {
        var gameAdquired = new GameAdquired
        {
            Id = game.GameId,
            Name = game.GameName
        };

        return await _repository.AddGameToLibraryAsync(game.GameId, gameAdquired);
    }

    public async Task<bool> RemoveGameFromLibraryAsync(RemoveGameFromLibraryDTO game)
    {
        return await _repository.RemoveGameFromLibraryAsync(game.UserId, game.GameId);
    }
}
