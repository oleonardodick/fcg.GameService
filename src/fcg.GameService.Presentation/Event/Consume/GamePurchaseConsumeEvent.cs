namespace fcg.GameService.Presentation.Event.Consume;

public class GamePurchaseConsumeEvent
{
    public Guid PaymentId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string GameId { get; set; } = string.Empty;
    public Guid CorrelationId { get; set; }
}
