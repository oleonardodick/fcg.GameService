using fcg.GameService.Presentation.DTOs.Game.Requests;
using fcg.GameService.Presentation.DTOs.Game.Responses;

namespace fcg.GameService.Application.Interfaces;

public interface IGameUseCase
{
    Task<IList<ResponseGameDTO>> GetAllAsync();
    Task<ResponseGameDTO?> GetByIdAsync(string id);
    Task<ResponseGameDTO> CreateAsync(CreateGameDTO request);
    Task<bool> UpdateAsync(string id, UpdateGameDTO request);
    Task<bool> UpdateTagsAsync(string id, UpdateTagsDTO tags);
    Task<bool> DeleteAsync(string id);
}
