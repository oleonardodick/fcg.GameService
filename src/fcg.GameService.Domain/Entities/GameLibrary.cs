namespace fcg.GameService.Domain.Entities;

public class GameLibrary : BaseEntity
{
    public string UserId { get; set; } = string.Empty;

    public List<GameAdquired> Games { get; set; } = [];

    public GameLibrary(string userId, List<GameAdquired> games)
    {
        UserId = userId;
        Games = games;
    }
}

public class GameAdquired
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public GameAdquired(string id, string name)
    {
        Id = id;
        Name = name;
    }
}
