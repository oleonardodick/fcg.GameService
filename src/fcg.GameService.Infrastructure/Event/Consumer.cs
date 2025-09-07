using fcg.GameService.Application.Interfaces;
using fcg.GameService.Domain.Event;

namespace fcg.GameService.Infrastructure.Event;

public class Consumer(IAppLogger<Consumer> logger) : IConsumer
{
    private readonly IAppLogger<Consumer> _logger = logger;

    public async Task<T> ConsumeAsync<T>() where T : class
    {
        // TODO: Implementar consumo real do evento (Service Bus, Event Grid, etc.)
        // Por enquanto, apenas logamos o evento
        _logger.LogInformation("Evento consumido");

        return await Task.FromResult(Activator.CreateInstance<T>());
    }
}
