using fcg.GameService.API.Entities;
using fcg.GameService.API.Infrastructure.Services;
using fcg.GameService.API.Repositories.Interfaces;

namespace fcg.GameService.API.Repositories.Implementations;

public class GameRepository : BaseRepository<Game>, IGameRepository
{
    public GameRepository(IMongoDbService mongoDbService) : base(mongoDbService) { }
}
