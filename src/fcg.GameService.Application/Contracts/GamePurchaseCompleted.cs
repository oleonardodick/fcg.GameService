namespace fcg.Contracts;

public record GamePurchaseCompleted
{
    public Guid PaymentId { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string GameId { get; init; } = string.Empty;
    public double Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string PaymentMethod { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string? Reason { get; init; }
    public Guid CorrelationId { get; init; }
    public DateTime CompletedAt { get; init; }
}
