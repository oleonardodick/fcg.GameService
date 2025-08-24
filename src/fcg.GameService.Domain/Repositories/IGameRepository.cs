using fcg.GameService.Domain.Entities;

namespace fcg.GameService.Domain.Repositories;

public interface IGameRepository : IBaseRepository<Game>
{
    Task<IList<Game>> GetAllAsync();
    Task<bool> UpdateAsync(Game game);
    Task<bool> UpdateTagsAsync(string id, List<string> tags);
}