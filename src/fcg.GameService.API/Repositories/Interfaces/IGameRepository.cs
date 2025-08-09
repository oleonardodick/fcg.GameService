using fcg.GameService.API.Entities;

namespace fcg.GameService.API.Repositories.Interfaces;

public interface IGameRepository : IBaseRepository<Game>
{
    Task<IList<Game>> GetAllAsync();
    Task UpdateAsync(Game game);
    Task UpdateTagsAsync(string id, string[] tags);
}
