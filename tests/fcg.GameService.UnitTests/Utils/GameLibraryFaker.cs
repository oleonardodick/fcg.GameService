using Bogus;
using fcg.GameService.Domain.Entities;
using MongoDB.Bson;

namespace fcg.GameService.UnitTests.Utils;

public static class GameLibraryFaker
{
    public static List<GameLibrary> FakeListOfGameLibrary(int qtToGenerate, int qtGames)
    {
        var gameLibraryFaker = new Faker<GameLibrary>()
            .CustomInstantiator(f => new GameLibrary(
                id: ObjectId.GenerateNewId().ToString(),
                userId: Guid.NewGuid().ToString(),
                games: FakeListOfGameAdquired(qtGames)
            ));
        return gameLibraryFaker.Generate(qtToGenerate);
    }

    public static List<GameAdquired> FakeListOfGameAdquired(int qtToGenerate) {
        var gameAdquiredFaker = new Faker<GameAdquired>()
            .CustomInstantiator(f => new GameAdquired(
                id: ObjectId.GenerateNewId().ToString(),
                name: f.Commerce.ProductName()
            ));

        return gameAdquiredFaker.Generate(qtToGenerate);
    }
}
