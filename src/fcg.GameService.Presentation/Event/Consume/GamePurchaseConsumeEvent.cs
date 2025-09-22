namespace fcg.GameService.Presentation.Event.Consume;

public class GamePurchaseConsumeEvent
{
    public Guid PaymentId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string GameId { get; set; } = string.Empty;
    public double Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public Guid CorrelationId { get; set; }
    public DateTime CompletedAt { get; set; }
}
