using fcg.GameService.Application.Helpers;
using fcg.GameService.Application.Interfaces;
using fcg.GameService.Application.Mappers.Adapters;
using fcg.GameService.Domain.Exceptions;
using fcg.GameService.Domain.Repositories;
using fcg.GameService.Presentation.DTOs.Game.Requests;
using fcg.GameService.Presentation.DTOs.Game.Responses;

namespace fcg.GameService.Application.UseCases;

public class GameUseCase : IGameUseCase
{
    private readonly IGameRepository _repository;
    private const string ENTITY = "Jogo";

    public GameUseCase(IGameRepository repository)
    {
        _repository = repository;
    }

    public async Task<IList<ResponseGameDTO>> GetAllAsync()
    {
        var games = await _repository.GetAllAsync();

        var response = GameMapperAdapter.FromListEntityToListDto(games);

        return response;
    }

    public async Task<ResponseGameDTO?> GetByIdAsync(string id)
    {
        var game = await _repository.GetByIdAsync(id);

        if (game == null)
            throw AppNotFoundException.ForEntity(ENTITY, id);

        var response = GameMapperAdapter.FromEntityToDto(game);

        return response;
    }

    public async Task<ResponseGameDTO> CreateAsync(CreateGameDTO request)
    {
        var game = GameMapperAdapter.FromDtoToEntity(request);

        var createdGame = await _repository.CreateAsync(game);

        var response = GameMapperAdapter.FromEntityToDto(createdGame);

        return response;
    }

    public async Task<bool> UpdateAsync(string id, UpdateGameDTO request)
    {
        var gameToUpdate = await _repository.GetByIdAsync(id);

        if (request.Tags is not null)
            request.Tags = TagHelper.NormalizeTags(request.Tags);

        gameToUpdate!.Update(
            request.Name,
            request.Price,
            request.ReleasedDate,
            request.Tags,
            request.Description
        );

        return await _repository.UpdateAsync(gameToUpdate!);
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
