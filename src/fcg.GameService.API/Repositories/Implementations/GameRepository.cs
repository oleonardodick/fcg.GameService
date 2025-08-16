using fcg.GameService.API.Entities;
using fcg.GameService.API.Infrastructure.Services;
using fcg.GameService.API.Repositories.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace fcg.GameService.API.Repositories.Implementations;

public class GameRepository : BaseRepository<Game>, IGameRepository
{
    public GameRepository(IMongoDbService mongoDbService) : base(mongoDbService) { }

    public async Task<IList<Game>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<bool> UpdateAsync(Game game)
    {
        var result = await _collection.ReplaceOneAsync(x => x.Id == game.Id, game);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> UpdateTagsAsync(string id, List<string> tags)
    {
        var _id = new ObjectId(id);
        var filter = Builders<Game>.Filter.Eq("_id", _id);
        var update = Builders<Game>.Update.Set("tags", tags);

        var result = await _collection.UpdateOneAsync(filter, update);

        return result.ModifiedCount > 0;      
    }
}
