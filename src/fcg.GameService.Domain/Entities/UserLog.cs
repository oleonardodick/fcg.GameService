namespace fcg.GameService.Domain.Entities;

public record UserLog(string UserId, string Tags, string GameId = "")
{
    public string UserId { get; } = UserId;
    public string Tags { get; } = Tags;
    public string GameId { get; } = GameId;
}
