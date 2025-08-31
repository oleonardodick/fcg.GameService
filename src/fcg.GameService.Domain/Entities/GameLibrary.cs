namespace fcg.GameService.Domain.Entities;

public record GameLibrary(string Id, string UserId, ICollection<GameAdquired> Games)
{
    public string Id { get; } = Id;
    public string UserId { get; } = UserId;
    public ICollection<GameAdquired> Games { get; } = Games;
}

public record GameAdquired(string Id, string Name, ICollection<string> Tags)
{
    public string Id { get; } = Id;
    public string Name { get; } = Name;
    public ICollection<string> Tags { get; } = Tags;
}
