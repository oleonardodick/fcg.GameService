namespace fcg.GameService.Domain.Event;

public interface IConsumer<T> where T : class
{
    Task<T> ConsumeAsync(CancellationToken cancellationToken);
}
