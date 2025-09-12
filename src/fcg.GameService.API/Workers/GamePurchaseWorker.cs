using fcg.GameService.Application.Interfaces;

namespace fcg.GameService.API.Workers;

public sealed class GamePurchaseWorker(
    IAppLogger<GamePurchaseWorker> _logger,
    IServiceScopeFactory factory) : BackgroundService
{
    private readonly IPurchaseUseCase _purchaseUseCase = factory.CreateScope().ServiceProvider.GetRequiredService<IPurchaseUseCase>();
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await _purchaseUseCase.ConsumeAsync(stoppingToken);
        }
    }
}
