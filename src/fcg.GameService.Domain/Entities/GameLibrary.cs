namespace fcg.GameService.Domain.Entities;

public class GameLibrary
{
    public string Id { get; private set; } = string.Empty;
    public string UserId { get; private set; } = string.Empty;

    public List<GameAdquired> Games { get; private set; } = [];

    public GameLibrary(string id, string userId, List<GameAdquired> games)
    {
        Id = id;
        UserId = userId;
        Games = games;
    }
}

public class GameAdquired
{
    public string Id { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public GameAdquired(string id, string name)
    {
        Id = id;
        Name = name;
    }
}
