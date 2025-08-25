using fcg.GameService.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace fcg.GameService.Infrastructure.Documents;

public class GameDocument : IDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("price")]
    public double Price { get; set; }

    [BsonElement("releasedData")]
    public DateTime ReleasedDate { get; set; }

    [BsonElement("tags")]
    public List<string> Tags { get; set; } = [];

    public static GameDocument FromDomain(Game game)
        => new GameDocument
        {
            Id = string.IsNullOrWhiteSpace(game.Id) ? ObjectId.GenerateNewId().ToString() : game.Id,
            Name = game.Name,
            Description = game.Description,
            Price = game.Price,
            ReleasedDate = game.ReleasedDate,
            Tags = game.Tags
        };

    public Game ToDomain()
        => new Game(Id, Name, Price, ReleasedDate, Tags, Description) { };
}
