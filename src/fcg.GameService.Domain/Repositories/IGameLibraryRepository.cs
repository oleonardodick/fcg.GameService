using fcg.GameService.Domain.Entities;

namespace fcg.GameService.Domain.Repositories;

public interface IGameLibraryRepository : IBaseRepository<GameLibrary>
{
    Task<GameLibrary?> GetByUserIdAsync(string userId);
    Task<bool> AddGameToLibraryAsync(string libraryId, GameAdquired game);
    Task<bool> RemoveGameFromLibraryAsync(string libraryId, string gameId);
    Task<bool> ExistsGameOnLibraryAsync(string libraryId, string gameId);
}
