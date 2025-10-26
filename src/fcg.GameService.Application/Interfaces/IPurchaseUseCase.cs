using fcg.GameService.Presentation.DTOs.Game.Requests;
using fcg.GameService.Presentation.DTOs.Game.Responses;

namespace fcg.GameService.Application.Interfaces;

public interface IPurchaseUseCase
{
    Task<ResponseQueuedDto> PublishAsync(PurchaseGameDTO request, CancellationToken cancellationToken);
    // Task ConsumeAsync(CancellationToken cancellationToken);
}
