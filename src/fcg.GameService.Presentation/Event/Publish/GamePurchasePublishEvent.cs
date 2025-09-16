namespace fcg.GameService.Presentation.Event.Publish;

public class GamePurchasePublishEvent
{
    public Guid PaymentId { get; } = Guid.NewGuid();
    public required string UserId { get; init; }
    public required string GameId { get; init; }
    public double Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public Guid CorrelationId { get; } = Guid.NewGuid();
}
