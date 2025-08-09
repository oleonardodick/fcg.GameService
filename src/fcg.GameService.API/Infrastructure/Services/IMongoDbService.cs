using MongoDB.Driver;

namespace fcg.GameService.API.Infrastructure.Services;

public interface IMongoDbService
{
    IMongoCollection<T> GetCollection<T>(string collectionName);
}
