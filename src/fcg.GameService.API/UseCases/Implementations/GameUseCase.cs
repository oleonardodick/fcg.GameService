using fcg.GameService.API.DTOs.Requests;
using fcg.GameService.API.DTOs.Responses;
using fcg.GameService.API.Helpers;
using fcg.GameService.API.Mappers;
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

        var response = GameMapper.FromListEntityToListDto(games);

        return response;
    }

    public async Task<ResponseGameDTO?> GetByIdAsync(string id)
    {
        var game = await _repository.GetByIdAsync(id);

        if (game == null)
            return null;

        var response = GameMapper.FromEntityToDto(game);

        return response;
    }

    public async Task<ResponseGameDTO> CreateAsync(CreateGameDTO request)
    {
        var game = GameMapper.FromDtoToEntity(request);

        await _repository.CreateAsync(game);

        var response = GameMapper.FromEntityToDto(game);

        return response;
    }

    public async Task<bool> UpdateAsync(string id, UpdateGameDTO request)
    {
        var gameToUpdate = await GetByIdAsync(id);

        if (gameToUpdate is null)
            return false;

        request.Name ??= gameToUpdate.Name;
        request.Description ??= gameToUpdate.Description;
        request.Price ??= gameToUpdate.Price;
        request.ReleasedDate ??= gameToUpdate.ReleasedDate;
        request.Tags = TagHelper.NormalizeTags(request.Tags ?? gameToUpdate.Tags);

        var game = GameMapper.FromDtoToUpdateEntity(request, id);

        return await _repository.UpdateAsync(game);
    }

    public async Task<bool> UpdateTagsAsync(string id, List<string> tags)
    {
        if (await GetByIdAsync(id) is null)
            return false;

        tags = TagHelper.NormalizeTags(tags);

        return await _repository.UpdateTagsAsync(id, tags);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        if (await GetByIdAsync(id) is null)
            return false;

        return await _repository.DeleteAsync(id);
    }
}
