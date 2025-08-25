using fcg.GameService.Domain.Entities;
using fcg.GameService.Domain.Repositories;
using fcg.GameService.Infrastructure.Documents;
using MongoDB.Driver;

namespace fcg.GameService.Infrastructure.Repositories;

public class GameLibraryRepository : BaseRepository<GameLibrary, GameLibraryDocument>, IGameLibraryRepository
{
    public GameLibraryRepository(IMongoDatabase database) 
        : base(database, "gameLibraries") { }

    public async Task<GameLibrary?> GetByUserIdAsync(string userId)
    {
        var filter = Builders<GameLibraryDocument>.Filter.Eq(g => g.UserId, userId);

        var result = await _collection.Find(filter).FirstOrDefaultAsync();

        return result == null ? default : ToDomain(result);
    }

    public async Task<bool> AddGameToLibraryAsync(string libraryId, GameAdquired game)
    {
        var gameDoc = GameAdquiredDocument.FromDomain(game);
        var filter = Builders<GameLibraryDocument>.Filter.Eq(g => g.Id, libraryId);
        var update = Builders<GameLibraryDocument>.Update.Push(g => g.Games, gameDoc);

        var result = await _collection.UpdateOneAsync(filter, update);

        return result.ModifiedCount > 0;
    }

    public async Task<bool> RemoveGameFromLibraryAsync(string libraryId, string gameId)
    {
        var filter = Builders<GameLibraryDocument>.Filter.Eq(g => g.Id, libraryId);
        var remove = Builders<GameLibraryDocument>.Update.PullFilter(g => g.Games, sub => sub.Id == gameId);

        var result = await _collection.UpdateOneAsync(filter, remove);

        return result.ModifiedCount > 0;
    }

    public async Task<bool> ExistsGameOnLibraryAsync(string libraryId, string gameId)
    {
        var filter = Builders<GameLibraryDocument>.Filter.And(
            Builders<GameLibraryDocument>.Filter.Eq(gl => gl.Id, libraryId),
            Builders<GameLibraryDocument>.Filter.ElemMatch(gl => gl.Games, g => g.Id == gameId)
        );

        return await _collection.Find(filter).AnyAsync();
    }

    protected override GameLibraryDocument ToDocument(GameLibrary domain)
    {
        return GameLibraryDocument.FromDomain(domain);
    }

    protected override GameLibrary ToDomain(GameLibraryDocument doc)
    {
        return doc.ToDomain();
    }
}
