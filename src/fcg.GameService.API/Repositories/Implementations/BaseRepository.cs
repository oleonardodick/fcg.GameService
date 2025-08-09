using fcg.GameService.API.Attributes;
using fcg.GameService.API.Entities;
using fcg.GameService.API.Infrastructure.Services;
using fcg.GameService.API.Repositories.Interfaces;
using MongoDB.Driver;

namespace fcg.GameService.API.Repositories.Implementations;

public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
{
    protected readonly IMongoCollection<T> _collection;

    public BaseRepository(IMongoDbService mongoDbService)
    {
        var attribute = Attribute.GetCustomAttribute(typeof(T), typeof(BsonCollectionAttribute)) as BsonCollectionAttribute;

        var collectionName =
            attribute?.CollectionName
            ??
            throw new InvalidOperationException($"A entidade {typeof(T).Name} deve conter o atributo [BsonCollection]");

        _collection = mongoDbService.GetCollection<T>(collectionName);
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        var result = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

        return result;
    }

    public async Task CreateAsync(T entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public async Task DeleteAsync(string id)
    {
        await _collection.DeleteOneAsync(x => x.Id == id);
    }
}
