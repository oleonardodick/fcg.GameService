namespace fcg.GameService.Domain.Entities;

public record GameLog(string GameId, string Tags)
{
    public string GameId { get; } = GameId;
    public string Tags { get; } = Tags;
}
