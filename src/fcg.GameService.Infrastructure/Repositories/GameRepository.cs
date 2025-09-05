using fcg.GameService.Domain.Entities;
using fcg.GameService.Domain.Repositories;
using fcg.GameService.Infrastructure.Documents;
using MongoDB.Bson;
using MongoDB.Driver;

namespace fcg.GameService.Infrastructure.Repositories;

public class GameRepository(IMongoDatabase database) : BaseRepository<Game, GameDocument>(database, "games"), IGameRepository
{
    public async Task<IList<Game>> GetAllAsync()
    {
        var docs = await _collection.Find(_ => true).ToListAsync();
        return docs.Select(ToDomain).ToList();
    }

    public async Task<IList<Game>> GetAllByIdsAsync(IEnumerable<string> ids)
    {
        var docs = await _collection.Find(d => ids.Contains(d.Id)).ToListAsync();
        return [.. docs.Select(ToDomain)];
    }

    public async Task<bool> UpdateAsync(Game game)
    {
        var gameDocument = ToDocument(game);
        var result = await _collection.ReplaceOneAsync(x => x.Id == game.Id, gameDocument);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> UpdateTagsAsync(string id, ICollection<string> tags)
    {
        var _id = new ObjectId(id);
        var filter = Builders<GameDocument>.Filter.Eq(d => d.Id, id);
        var update = Builders<GameDocument>.Update.Set(d => d.Tags, tags);

        var result = await _collection.UpdateOneAsync(filter, update);

        return result.ModifiedCount > 0;      
    }

    protected override GameDocument ToDocument(Game domain)
    {
        return GameDocument.FromDomain(domain);
    }

    protected override Game ToDomain(GameDocument doc)
    {
        return doc.ToDomain();
    }
}
