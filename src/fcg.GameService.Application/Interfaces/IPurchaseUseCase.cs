using fcg.GameService.Presentation.DTOs.Game.Requests;
using fcg.GameService.Presentation.DTOs.Game.Responses;

namespace fcg.GameService.Application.Interfaces;

public interface IPurchaseUseCase
{
    Task<ResponseQueuedDto> PublishAsync(PurchaseGameDTO request);
    Task<ConsumedQueueDto?> ConsumeAsync();
}
