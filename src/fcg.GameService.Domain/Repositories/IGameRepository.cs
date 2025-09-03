using fcg.GameService.Domain.Entities;

namespace fcg.GameService.Domain.Repositories;

public interface IGameRepository : IBaseRepository<Game>
{
    Task<IList<Game>> GetAllAsync();
    Task<IList<Game>> GetAllByIdsAsync(IEnumerable<string> ids);
    Task<bool> UpdateAsync(Game game);
    Task<bool> UpdateTagsAsync(string id, ICollection<string> tags);
}