namespace fcg.GameService.Domain.Repositories;

public interface IBaseRepository<T>
{
    Task<T?> GetByIdAsync(string id);
    Task<T> CreateAsync(T entity);
    Task<bool> DeleteAsync(string id);
}
