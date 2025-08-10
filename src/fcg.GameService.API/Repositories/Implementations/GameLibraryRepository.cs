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

    public async Task<bool> AddGameToLibraryAsync(string userId, GameAdquired game)
    {
        var filter = Builders<GameLibrary>.Filter.Eq(g => g.UserId, userId);
        var update = Builders<GameLibrary>.Update.Push(g => g.Games, game);

        var result = await _collection.UpdateOneAsync(filter, update);

        return true;
    }

    public async Task<bool> RemoveGameFromLibraryAsync(string userId, string gameId)
    {
        var filter = Builders<GameLibrary>.Filter.Eq(g => g.UserId, userId);
        var remove = Builders<GameLibrary>.Update.PullFilter(g => g.Games, sub => sub.Id == gameId);

        var result = await _collection.UpdateOneAsync(filter, remove);

        return true;
    }
}
