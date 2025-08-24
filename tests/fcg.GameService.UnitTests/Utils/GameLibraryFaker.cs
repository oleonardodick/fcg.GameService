using Bogus;
using fcg.GameService.Domain.Entities;

namespace fcg.GameService.UnitTests.Utils;

public static class GameLibraryFaker
{
    public static List<GameLibrary> FakeListOfGameLibrary(int qtToGenerate)
    {
        var gameLibraryFaker = new Faker<GameLibrary>()
            .CustomInstantiator(f => new GameLibrary(
                userId: Guid.NewGuid().ToString(),
                games: FakeListOfGameAdquired(f.Random.Int(1, 5))
            ));
        return gameLibraryFaker.Generate(qtToGenerate);
    }

    public static List<GameAdquired> FakeListOfGameAdquired(int qtToGenerate) {
        var gameAdquiredFaker = new Faker<GameAdquired>()
            .CustomInstantiator(f => new GameAdquired(
                id: Guid.NewGuid().ToString(),
                name: f.Commerce.ProductName()
            ));

        return gameAdquiredFaker.Generate(qtToGenerate);
    }
}
