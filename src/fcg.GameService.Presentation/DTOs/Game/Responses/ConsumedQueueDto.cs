namespace fcg.GameService.Presentation.DTOs.Game.Responses;

public class ConsumedQueueDto
{
    public Guid PaymentId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string GameId { get; set; } = string.Empty;
    public Guid CorrelationId { get; set; }
}
