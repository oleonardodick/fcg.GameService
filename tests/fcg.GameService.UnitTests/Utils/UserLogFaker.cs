using Bogus;
using fcg.GameService.Domain.Entities;

namespace fcg.GameService.UnitTests.Utils;

public static class UserLogFaker
{
    public static IReadOnlyCollection<UserLog> FakeListOfUserLog(
        List<GameLibrary> library, 
        List<Game> games, 
        int qtToGenerate)
    {
        var userLogFaker = new Faker<UserLog>()
            .CustomInstantiator(f =>
            {
                var lib = f.PickRandom(library);
                var userId = lib.UserId;
                var gameIds = lib.Games.Select(g => g.Id).ToList();
                var tags = games
                    .Where(g => gameIds.Contains(g.Id))
                    .SelectMany(g => g.Tags)
                    .Distinct();
                var tagsString = string.Join("|", tags);
                return new UserLog(userId, tagsString);
            });

        return userLogFaker.Generate(qtToGenerate);
    }
}
