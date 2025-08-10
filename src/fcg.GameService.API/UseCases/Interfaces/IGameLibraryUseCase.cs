using fcg.GameService.API.DTOs.GameLibrary;
using fcg.GameService.API.DTOs.GameLibrary.Requests;
using fcg.GameService.API.DTOs.Responses;
using fcg.GameService.API.Entities;

namespace fcg.GameService.API.UseCases.Interfaces;

public interface IGameLibraryUseCase
{
    Task<ResponseGameLibraryDTO> GetByUserIdAsync(string userId);
    Task<ResponseGameLibraryDTO> CreateAsync(CreateGameLibraryDTO request);
    Task<bool> AddGameToLibraryAsync(AddGameToLibraryDTO game);
    Task<bool> RemoveGameFromLibraryAsync(RemoveGameFromLibraryDTO game);
}
