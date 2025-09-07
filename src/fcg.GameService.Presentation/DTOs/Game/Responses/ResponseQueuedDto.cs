using fcg.GameService.Domain.Enums;

namespace fcg.GameService.Presentation.DTOs.Game.Responses;

public class ResponseQueuedDto
{
    public Guid PaymentId { get; set; }
    public QueueStatus Status { get; set; }
    public bool IsQueued { get; set; }
    public string Message { get; set; } = string.Empty;
}
