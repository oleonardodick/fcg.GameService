using fcg.GameService.API.DTOs.GameLibrary;
using fcg.GameService.API.DTOs.GameLibrary.Requests;
using fcg.GameService.API.DTOs.Responses;

namespace fcg.GameService.API.UseCases.Interfaces;

public interface IGameLibraryUseCase
{
    Task<ResponseGameLibraryDTO?> GetByIdAsync(string id);
    Task<ResponseGameLibraryDTO?> GetByUserIdAsync(string userId);
    Task<ResponseGameLibraryDTO> CreateAsync(CreateGameLibraryDTO request);
    Task<bool> ExistsGameOnLibraryAsync(string libraryId, string gameId);
    Task<bool> AddGameToLibraryAsync(string libraryId, AddGameToLibraryDTO game);
    Task<bool> RemoveGameFromLibraryAsync(string libraryId, RemoveGameFromLibraryDTO game);
}
