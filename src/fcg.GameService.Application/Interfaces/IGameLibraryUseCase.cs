using fcg.GameService.Presentation.DTOs.GameLibrary.Requests;
using fcg.GameService.Presentation.DTOs.GameLibrary.Responses;

namespace fcg.GameService.Application.Interfaces;

public interface IGameLibraryUseCase
{
    Task<ResponseGameLibraryDTO?> GetByIdAsync(string id);
    Task<ResponseGameLibraryDTO?> TryGetByUserIdAsync(string userId);
    Task<ResponseGameLibraryDTO> GetByUserIdAsync(string userId);
    Task<ResponseGameLibraryDTO> CreateAsync(CreateGameLibraryDTO request);
    Task<bool> ExistsGameOnLibraryAsync(string libraryId, string gameId);
    Task<bool> AddGameToLibraryAsync(string libraryId, AddGameToLibraryDTO game);
    Task<bool> RemoveGameFromLibraryAsync(string libraryId, RemoveGameFromLibraryDTO game);
}
