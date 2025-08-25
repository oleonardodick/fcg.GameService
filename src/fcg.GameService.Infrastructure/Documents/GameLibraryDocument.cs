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
    public List<GameAdquiredDocument> Games { get; set; } = [];

    public static GameLibraryDocument FromDomain(GameLibrary library)
        => new GameLibraryDocument
        {
            Id = string.IsNullOrWhiteSpace(library.Id) ? ObjectId.GenerateNewId().ToString() : library.Id,
            UserId = library.UserId,
            Games = library.Games.Select(GameAdquiredDocument.FromDomain).ToList()
        };

    public GameLibrary ToDomain()
        => new GameLibrary(
            Id,
            UserId,
            Games.Select(g => g.ToDomain()).ToList()
            )
        {  };
}

public class GameAdquiredDocument
{
    [BsonElement("id")]
    public string Id { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    public static GameAdquiredDocument FromDomain(GameAdquired adquired)
        => new GameAdquiredDocument
        {
            Id = adquired.Id,
            Name = adquired.Name,
        };

    public GameAdquired ToDomain()
        => new GameAdquired(Id, Name);
}
