using fcg.GameService.API.Entities;
using fcg.GameService.API.Infrastructure.Services;
using fcg.GameService.API.Repositories.Interfaces;
using MongoDB.Driver;

namespace fcg.GameService.API.Repositories.Implementations;

public class GameLibraryRepository : BaseRepository<GameLibrary>, IGameLibraryRepository
{
    public GameLibraryRepository(IMongoDbService mongoDbService) : base(mongoDbService) { }

    public async Task<GameLibrary?> GetByUserIdAsync(string userId)
    {
        var filter = Builders<GameLibrary>.Filter.Eq(g => g.UserId, userId);

        var result = await _collection.Find(filter).FirstOrDefaultAsync();

        return result;
    }

    public async Task<bool> AddGameToLibraryAsync(string libraryId, GameAdquired game)
    {
        var filter = Builders<GameLibrary>.Filter.Eq(g => g.Id, libraryId);
        var update = Builders<GameLibrary>.Update.Push(g => g.Games, game);

        var result = await _collection.UpdateOneAsync(filter, update);

        return result.ModifiedCount > 0;
    }

    public async Task<bool> RemoveGameFromLibraryAsync(string libraryId, string gameId)
    {
        var filter = Builders<GameLibrary>.Filter.Eq(g => g.Id, libraryId);
        var remove = Builders<GameLibrary>.Update.PullFilter(g => g.Games, sub => sub.Id == gameId);

        var result = await _collection.UpdateOneAsync(filter, remove);

        return result.ModifiedCount > 0;
    }
}
