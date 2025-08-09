using fcg.GameService.API.DTOs.Requests;
using fcg.GameService.API.DTOs.Responses;

namespace fcg.GameService.API.UseCases.Interfaces;

public interface IGameUseCase
{
    Task<IList<ResponseGameDTO>> GetAllAsync();
    Task<ResponseGameDTO?> GetByIdAsync(string id);
    Task<ResponseGameDTO> CreateAsync(CreateGameDTO request);
    Task<bool> UpdateAsync(string id, UpdateGameDTO request);
    Task<bool> DeleteAsync(string id);
}
