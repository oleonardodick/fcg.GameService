namespace fcg.GameService.Domain.Entities;

public class Game
{
    public string Id { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public double Price { get; private set; }

    public DateTime ReleasedDate { get; private set; }

    public List<string> Tags { get; private set; } = [];

    public Game(string id, string name, double price, DateTime releasedDate, List<string> tags, string? description)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
        ReleasedDate = releasedDate;
        Tags = tags;
    }

    public void Update(string? name, double? price, DateTime? releasedDate, List<string>? tags, string? description)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name;

        if (!string.IsNullOrWhiteSpace(description))
            Description = description;

        if (price.HasValue)
            Price = price.Value;

        if (releasedDate.HasValue)
            ReleasedDate = releasedDate.Value;

        if (tags is not null)
            Tags = tags;
    }
}
