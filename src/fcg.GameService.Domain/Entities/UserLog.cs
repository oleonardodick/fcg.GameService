namespace fcg.GameService.Domain.Entities;

public record UserLog(string UserId, string Tags)
{
    public string UserId { get; } = UserId;
    public string Tags { get; } = Tags;
}
