using fcg.GameService.Domain.Entities;

namespace fcg.GameService.UnitTests.Utils;

public static class GameLogFaker
{
    public static IReadOnlyCollection<GameLog> FakeListOfGameLog(List<Game> games)
    {
        return [.. games.Select(g => new GameLog(
           GameId: g.Id,
           Tags: string.Join("|", g.Tags)
        ))];
    }
}
