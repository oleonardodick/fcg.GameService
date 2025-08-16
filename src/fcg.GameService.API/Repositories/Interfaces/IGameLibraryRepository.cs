using fcg.GameService.API.Entities;

namespace fcg.GameService.API.Repositories.Interfaces;

public interface IGameLibraryRepository : IBaseRepository<GameLibrary>
{
    Task<GameLibrary?> GetByUserIdAsync(string userId);
    Task<bool> AddGameToLibraryAsync(string libraryId, GameAdquired game);
    Task<bool> RemoveGameFromLibraryAsync(string libraryId, string gameId);
}
