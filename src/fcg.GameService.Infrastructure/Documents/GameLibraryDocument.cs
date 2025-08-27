using fcg.GameService.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace fcg.GameService.Infrastructure.Documents;

public class GameLibraryDocument : IDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("userId")]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("games")]
    public ICollection<GameAdquiredDocument> Games { get; set; } = [];

    public static GameLibraryDocument FromDomain(GameLibrary library)
        => new()
        {
            Id = string.IsNullOrWhiteSpace(library.Id) ? ObjectId.GenerateNewId().ToString() : library.Id,
            UserId = library.UserId,
            Games = [.. library.Games.Select(GameAdquiredDocument.FromDomain)]
        };

    public GameLibrary ToDomain()
        => new(
            Id,
            UserId,
            [.. Games.Select(g => g.ToDomain())]
            )
        { };
}

public class GameAdquiredDocument
{
    [BsonElement("id")]
    public string Id { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("tags")]
    public ICollection<string> Tags { get; set; } = [];

    public static GameAdquiredDocument FromDomain(GameAdquired adquired)
        => new()
        {
            Id = adquired.Id,
            Name = adquired.Name,
            Tags = adquired.Tags
        };

    public GameAdquired ToDomain()
        => new(Id, Name, Tags);
}
