namespace fcg.GameService.Domain.Entities;

public class Game(string id, string name, double price, DateTime releasedDate, ICollection<string> tags, string? description)
{
    public string Id { get; private set; } = id;
    public string Name { get; private set; } = name;

    public string? Description { get; private set; } = description;

    public double Price { get; private set; } = price;

    public DateTime ReleasedDate { get; private set; } = releasedDate;

    public ICollection<string> Tags { get; private set; } = tags;

    public void Update(string? name, double? price, DateTime? releasedDate, ICollection<string>? tags, string? description)
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
