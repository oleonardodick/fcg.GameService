using fcg.GameService.API.Entities;

namespace fcg.GameService.API.Repositories.Interfaces;

public interface IGameRepository : IBaseRepository<Game>
{
    Task<IList<Game>> GetAllAsync();
    Task<bool> UpdateAsync(Game game);
    Task<bool> UpdateTagsAsync(string id, string[] tags);
}
