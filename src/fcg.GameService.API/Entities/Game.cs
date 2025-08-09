using fcg.GameService.API.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace fcg.GameService.API.Entities;

[BsonCollection("games")]
public class Game : BaseEntity
{
    [BsonElement("name")]
    public required string Name { get; set; }

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("price")]
    public double Price { get; set; }

    [BsonElement("releasedData")]
    public DateTime ReleasedDate { get; set; }

    [BsonElement("tags")]
    public required string[] Tags { get; set; }
}
