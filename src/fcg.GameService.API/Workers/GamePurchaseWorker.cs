using fcg.GameService.Application.Interfaces;

namespace fcg.GameService.API.Workers;

public sealed class GamePurchaseWorker(
    IAppLogger<GamePurchaseWorker> _logger,
    IPurchaseUseCase _purchaseUseCase) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await _purchaseUseCase.ConsumeAsync(stoppingToken);
        }
    }
}
