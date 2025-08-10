using fcg.GameService.API.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace fcg.GameService.API.Entities;

[BsonCollection("gameLibraries")]
public class GameLibrary : BaseEntity
{
    [BsonElement("userId")]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("games")]
    public List<GameAdquired> Games { get; set; } = [];
}

public class GameAdquired
{
    [BsonElement("id")]
    public string Id { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;
}
