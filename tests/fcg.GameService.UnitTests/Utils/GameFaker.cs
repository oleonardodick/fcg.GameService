using Bogus;
using fcg.GameService.Domain.Entities;
using MongoDB.Bson;

namespace fcg.GameService.UnitTests.Utils;

public static class GameFaker
{
    public static List<Game> FakeListOfGame(int qtToGenerate)
        {
        var gameFaker = new Faker<Game>()
            .CustomInstantiator(f => new Game(
                id: ObjectId.GenerateNewId().ToString(),
                name: f.Commerce.ProductName(),
                price: f.Random.Double(100, 500),
                releasedDate: f.Date.Recent(30),
                tags: f.Lorem.Words(f.Random.Int(1, 5)).ToList(),
                description: f.Lorem.Sentence()
            ));
            
            return gameFaker.Generate(qtToGenerate);
        }
}
