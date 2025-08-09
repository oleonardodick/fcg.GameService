using fcg.GameService.API.Entities;

namespace fcg.GameService.API.Repositories.Interfaces;

public interface IBaseRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(string id);
    Task CreateAsync(T entity);
    Task DeleteAsync(string id);
}
