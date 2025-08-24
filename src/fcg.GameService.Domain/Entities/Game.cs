namespace fcg.GameService.Domain.Entities;

public class Game : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public double Price { get; set; }

    public DateTime ReleasedDate { get; set; }

    public List<string> Tags { get; set; } = [];

    public Game(string name, double price, DateTime releasedDate, List<string> tags, string? description)
    {
        Name = name;
        Description = description;
        Price = price;
        ReleasedDate = releasedDate;
        Tags = tags;
    }
}
