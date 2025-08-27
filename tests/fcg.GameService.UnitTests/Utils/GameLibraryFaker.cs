using Bogus;
using fcg.GameService.Domain.Entities;
using MongoDB.Bson;

namespace fcg.GameService.UnitTests.Utils;

public static class GameLibraryFaker
{
    public static List<GameLibrary> FakeListOfGameLibrary(int qtToGenerate, int qtGames)
    {
        Faker<GameLibrary> gameLibraryFaker = new Faker<GameLibrary>()
            .CustomInstantiator(f => new GameLibrary(
                Id: ObjectId.GenerateNewId().ToString(),
                UserId: Guid.NewGuid().ToString(),
                Games: FakeListOfGameAdquired(qtGames)
            ));
        return gameLibraryFaker.Generate(qtToGenerate);
    }

    public static List<GameAdquired> FakeListOfGameAdquired(int qtToGenerate)
    {
        Faker<GameAdquired> gameAdquiredFaker = new Faker<GameAdquired>()
            .CustomInstantiator(f => new GameAdquired(
                Id: ObjectId.GenerateNewId().ToString(),
                Name: f.Commerce.ProductName(),
                Tags: [.. f.Lorem.Words(f.Random.Int(1, 5))]
            ));

        return gameAdquiredFaker.Generate(qtToGenerate);
    }
}
