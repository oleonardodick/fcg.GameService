namespace fcg.GameService.Domain.Event;

public interface IConsumer
{
    Task<T> ConsumeAsync<T>() where T : class;
}
