using Bogus;
using fcg.GameService.API.Entities;
using MongoDB.Bson;

namespace fcg.GameService.UnitTests.Utils;

public static class GameLibraryFaker
{
    public static List<GameLibrary> FakeListOfGameLibrary(int qtToGenerate)
    {
        var gameLibraryFaker = new Faker<GameLibrary>()
            .RuleFor(gl => gl.UserId, f => ObjectId.GenerateNewId().ToString())
            .RuleFor(gl => gl.Games, f => FakeListOfGameAdquired(f.Random.Int(1, 5)));

        return gameLibraryFaker.Generate(qtToGenerate);
    }

    public static List<GameAdquired> FakeListOfGameAdquired(int qtToGenerate) {
        var gameAdquiredFaker = new Faker<GameAdquired>()
            .RuleFor(g => g.Id, f => ObjectId.GenerateNewId().ToString())
            .RuleFor(g => g.Name, f => f.Commerce.ProductName());

        return gameAdquiredFaker.Generate(qtToGenerate);
    }
}
