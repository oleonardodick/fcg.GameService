using fcg.GameService.Domain.Enums;

namespace fcg.GameService.Presentation.DTOs.Game.Requests;

public class PurchaseGameDTO
{
    public string UserId { get; set; } = string.Empty;
    public string GameId { get; set; } = string.Empty;
    public CurrencyCode Currency { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
}
