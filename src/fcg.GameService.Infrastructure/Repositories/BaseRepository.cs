using fcg.GameService.Domain.Repositories;
using fcg.GameService.Infrastructure.Documents;
using MongoDB.Driver;

namespace fcg.GameService.Infrastructure.Repositories;

public abstract class BaseRepository<TDomain, TDocument> : IBaseRepository<TDomain> 
    where TDocument : IDocument
{
    protected readonly IMongoCollection<TDocument> _collection;

    public BaseRepository(IMongoDatabase database, string collectionName)
    {
        _collection = database.GetCollection<TDocument>(collectionName);
    }

    protected abstract TDocument ToDocument(TDomain domain);
    protected abstract TDomain ToDomain(TDocument doc);

    public async Task<TDomain?> GetByIdAsync(string id)
    {
        var result = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

        return result == null ? default : ToDomain(result);
    }

    public async Task<TDomain> CreateAsync(TDomain entity)
    {
        var document = ToDocument(entity);
        await _collection.InsertOneAsync(document);

        return ToDomain(document);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _collection.DeleteOneAsync(x => x.Id == id);

        return result.DeletedCount > 0;
    }
}
