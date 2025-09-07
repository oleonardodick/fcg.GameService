using fcg.GameService.Application.Interfaces;
using fcg.GameService.Domain.Event;

namespace fcg.GameService.Infrastructure.Event;

public class Publisher(IAppLogger<Publisher> logger) : IPublisher
{
    private readonly IAppLogger<Publisher> _logger = logger;

    public async Task PublishAsync<T>(T @event) where T : class
    {
        // TODO: Implementar publicação real do evento (Service Bus, Event Grid, etc.)
        // Por enquanto, apenas logamos o evento
        _logger.LogInformation("Evento publicado: {Event}", @event);
    }
}
